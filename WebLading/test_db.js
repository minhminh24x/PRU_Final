import "dotenv/config";
import { getMongo } from "./db.js";

async function run() {
  try {
    const { client, db } = await getMongo();
    const users = await db.collection("Users").find({}).limit(2).toArray();
    console.log(JSON.stringify(users, null, 2));
    await client.close();
  } catch (err) {
    console.error(err);
  }
}
run();
