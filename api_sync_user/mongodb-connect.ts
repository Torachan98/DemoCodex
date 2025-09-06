import { Collection, Db, MongoClient } from 'mongodb';

type QueryParams = {
  nameCollection: string;
  client: MongoClient;
  context: string;
};

const dbCache = new WeakMap<MongoClient, Map<string, Db>>();

const getDb = async (client: MongoClient, context: string): Promise<Db> => {
  let clientCache = dbCache.get(client);

  if (!clientCache) {
    clientCache = new Map<string, Db>();
    dbCache.set(client, clientCache);
  }

  let db = clientCache.get(context);
  if (!db) {
    db = client.db(context);
    clientCache.set(context, db);
  }

  return db;
};

const query = async <T = unknown>({
  nameCollection,
  client,
  context,
}: QueryParams): Promise<Collection<T>> => {
  const db = await getDb(client, context);
  return db.collection<T>(nameCollection);
};

export { query };
