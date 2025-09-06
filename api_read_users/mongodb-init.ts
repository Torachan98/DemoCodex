import { Collection, Db, MongoClient } from "mongodb";

type QueryParams = {
    nameCollection: string;
}

const uri = `mongodb://${process.env.DB_HOST}:${process.env.DB_PORT}/?authSource=${process.env.DB_CONTEXT}`;
const client = new MongoClient(uri, { monitorCommands: true });
let db: Db;

const connected = async () => {
  if (!db) {
    await client.connect();
    db = client.db(process.env.DB_CONTEXT);
  }
  return db;
};

const query = async <T = unknown>({ nameCollection }: QueryParams): Promise<Collection<T>> => {
    try {
        const context = await connected();
        return context.collection<T>(nameCollection);
    }
    catch(ex) {
        throw ex;
    }
}

export { client, query };