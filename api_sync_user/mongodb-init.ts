import { MongoClient } from 'mongodb';

type ConnectStringParams = {
  host: string;
  port: string;
  context: string;
};

const createInstance = ({ host, port, context }: ConnectStringParams): MongoClient => {
  try {
    const uri = `mongodb://${host}:${port}/?authSource=${context}`;
    const client = new MongoClient(uri, { monitorCommands: true });

    return client;
  } catch (ex) {
    throw ex;
  }
};

export { createInstance };
