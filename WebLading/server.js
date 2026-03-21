import "dotenv/config";
import path from "node:path";
import { fileURLToPath } from "node:url";
import fs from "node:fs/promises";

import bcrypt from "bcryptjs";
import express from "express";
import session from "express-session";
import helmet from "helmet";
import { getMongo, isMongoEnabled } from "./db.js";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const app = express();
const PORT = process.env.PORT ? Number(process.env.PORT) : 3000;

app.set("view engine", "ejs");
app.set("views", path.join(__dirname, "views"));

app.use(helmet({ contentSecurityPolicy: false }));
app.use(express.urlencoded({ extended: false }));
app.use(express.static(path.join(__dirname, "public")));
app.use(
  session({
    name: "weblading.sid",
    secret: process.env.SESSION_SECRET || "dev-secret-change-me",
    resave: false,
    saveUninitialized: false,
    cookie: { httpOnly: true, sameSite: "lax" }
  })
);

async function readUsers() {
  const p = path.join(__dirname, "data", "users.json");
  const raw = await fs.readFile(p, "utf-8");
  const data = JSON.parse(raw);
  if (!data || !Array.isArray(data.users)) throw new Error("Invalid users.json");
  return data.users;
}

function safeUserView(user) {
  const { id, username, displayName, level, exp, coin, hp, mp, stats, lastLoginAt, createdAt, raw } =
    user;
  return {
    id,
    username,
    displayName,
    level,
    exp,
    coin,
    hp,
    mp,
    stats,
    lastLoginAt,
    createdAt,
    raw
  };
}

function safeProgressView(progress) {
  if (!progress) return null;
  const {
    CurrentLevel,
    CurrentScene,
    PlayerX,
    PlayerY,
    Coins,
    Score,
    CurrentHealth,
    MaxHealth,
    EnemiesKilled,
    TotalPlayTime
  } = progress;
  return {
    currentLevel: CurrentLevel,
    currentScene: CurrentScene,
    position: { x: PlayerX, y: PlayerY },
    coins: Coins,
    score: Score,
    hp: { current: CurrentHealth, max: MaxHealth },
    enemiesKilled: EnemiesKilled,
    totalPlayTime: TotalPlayTime
  };
}

function toWebUserFromMongo(userDoc, progressDoc) {
  const userId = userDoc?._id?.toString?.() || null;
  const username = userDoc?.Username ?? userDoc?.username ?? null;
  const progress = safeProgressView(progressDoc);

  const stats = {
    score: progress?.score ?? 0,
    enemiesKilled: progress?.enemiesKilled ?? 0,
    totalPlayTime: progress?.totalPlayTime ?? 0,
    currentScene: progress?.currentScene ?? "-"
  };

  return {
    id: userId,
    username,
    displayName: userDoc?.Email ?? null,
    level: progress?.currentLevel ?? 1,
    exp: null,
    coin: progress?.coins ?? 0,
    hp: progress?.hp ?? { current: 0, max: 0 },
    mp: null,
    stats,
    lastLoginAt: null,
    createdAt: null,
    raw: {
      user: userDoc,
      progress: progressDoc
    }
  };
}

async function getUserAndProgressFromMongoByCredentials(username, password) {
  const { db } = await getMongo();
  const users = db.collection("Users");
  const progress = db.collection("PlayerProgress");

  const userDoc = await users.findOne({
    $and: [{ Username: username }, { Password: password }]
  });

  if (!userDoc) return null;

  const userId = userDoc._id?.toString?.();
  const progressDoc = userId ? await progress.findOne({ UserId: userId }) : null;
  return { userDoc, progressDoc };
}

async function getUserAndProgressFromMongoByUserId(userId) {
  const { db } = await getMongo();
  const users = db.collection("Users");
  const progress = db.collection("PlayerProgress");

  const { ObjectId } = await import("mongodb");
  const userDoc = await users.findOne({ _id: new ObjectId(userId) });
  if (!userDoc) return null;
  const progressDoc = await progress.findOne({ UserId: userId });
  return { userDoc, progressDoc };
}

function requireAuth(req, res, next) {
  if (!req.session?.userId) return res.redirect("/login");
  next();
}

app.get("/", (req, res) => {
  if (req.session?.userId) return res.redirect("/me");
  res.render("index");
});

app.get("/login", (req, res) => {
  if (req.session?.userId) return res.redirect("/me");
  res.render("login", { error: null, mongoEnabled: isMongoEnabled() });
});

app.get("/download", (req, res) => {
  res.render("download");
});

async function authenticateLocal(username, password) {
  const users = await readUsers();
  const user = users.find((u) => String(u.username).toLowerCase() === username.toLowerCase());
  if (!user) return null;

  const ok = await bcrypt.compare(password, user.passwordHash);
  if (!ok) return null;
  return user;
}

app.post("/login", async (req, res) => {
  const username = String(req.body?.username || "").trim();
  const password = String(req.body?.password || "");

  if (!username || !password) {
    return res
      .status(400)
      .render("login", { error: "Vui lòng nhập đủ username và password.", mongoEnabled: isMongoEnabled() });
  }

  if (isMongoEnabled()) {
    try {
      const result = await getUserAndProgressFromMongoByCredentials(username, password);
      if (result) {
        req.session.userId = result.userDoc._id.toString();
        return res.redirect("/me");
      }
      // If result is null, it means wrong credentials in Mongo
      return res
        .status(401)
        .render("login", { error: "Sai username hoặc password (MongoDB).", mongoEnabled: true });
    } catch (e) {
      console.error("MongoDB connection error, falling back to local:", e.message);
      // Fallback to local
      const user = await authenticateLocal(username, password);
      if (user) {
        req.session.userId = user.id;
        return res.redirect("/me");
      }
      return res.status(401).render("login", {
        error: `Lỗi kết nối MongoDB (${e.message}). Đăng nhập local cũng thất bại.`,
        mongoEnabled: true
      });
    }
  } else {
    const user = await authenticateLocal(username, password);
    if (!user) {
      return res.status(401).render("login", { error: "Sai username hoặc password.", mongoEnabled: false });
    }
    req.session.userId = user.id;
    return res.redirect("/me");
  }
});

app.post("/logout", (req, res) => {
  req.session.destroy(() => res.redirect("/login"));
});

app.get("/me", requireAuth, async (req, res) => {
  if (isMongoEnabled()) {
    try {
      const { db } = await getMongo();
      const { ObjectId } = await import("mongodb");
      const users = db.collection("Users");
      const progress = db.collection("PlayerProgress");

      const userDoc = await users.findOne({ _id: new ObjectId(req.session.userId) });
      if (!userDoc) {
        req.session.destroy(() => res.redirect("/login"));
        return;
      }
      const progressDoc = await progress.findOne({ UserId: req.session.userId });
      const view = toWebUserFromMongo(userDoc, progressDoc);
      res.render("me", { user: safeUserView(view) });
    } catch {
      req.session.destroy(() => res.redirect("/login"));
    }
    return;
  }

  const users = await readUsers();
  const user = users.find((u) => u.id === req.session.userId);
  if (!user) {
    req.session.destroy(() => res.redirect("/login"));
    return;
  }
  res.render("me", { user: safeUserView(user) });
});

app.get("/api/me", requireAuth, async (req, res) => {
  if (isMongoEnabled()) {
    try {
      const { db } = await getMongo();
      const { ObjectId } = await import("mongodb");
      const users = db.collection("Users");
      const progress = db.collection("PlayerProgress");
      const userDoc = await users.findOne({ _id: new ObjectId(req.session.userId) });
      if (!userDoc) return res.status(404).json({ error: "User not found" });
      const progressDoc = await progress.findOne({ UserId: req.session.userId });
      return res.json(safeUserView(toWebUserFromMongo(userDoc, progressDoc)));
    } catch (e) {
      return res.status(500).json({ error: e.message });
    }
  }

  const users = await readUsers();
  const user = users.find((u) => u.id === req.session.userId);
  if (!user) return res.status(404).json({ error: "User not found" });
  res.json(safeUserView(user));
});

app.listen(PORT, () => {
  console.log(`WebLading running on http://localhost:${PORT}`);
});

