import { MongoClient, ObjectId } from "mongodb";

let clientPromise = null;

export function isMongoEnabled() {
  return Boolean(process.env.MONGODB_URI);
}

export async function getMongo() {
  const uri = process.env.MONGODB_URI;
  if (!uri) throw new Error("MONGODB_URI is not set");

  if (!clientPromise) {
    const tlsAllowInvalid = String(process.env.MONGODB_TLS_INSECURE || "").toLowerCase() === "true";
    const client = new MongoClient(uri, {
      ...(tlsAllowInvalid ? { tlsAllowInvalidCertificates: true } : {}),
      connectTimeoutMS: 5000, // Timeout after 5 seconds
      serverSelectionTimeoutMS: 5000 // Timeout for server discovery
    });
    clientPromise = client.connect();
  }

  const client = await clientPromise;
  const dbName = process.env.MONGODB_DB || "GameDB";
  return { client, db: client.db(dbName) };
}

export function toObjectId(maybeId) {
  try {
    if (!maybeId) return null;
    return new ObjectId(String(maybeId));
  } catch {
    return null;
  }
}

