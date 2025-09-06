import { MongoClient } from 'mongodb';
import { query } from '../../mongodb-connect';
import { User } from '../interfaces/user.interface';
import { createDate } from '../shared/utility';

type Params = {
  nameCollection: string;
  client: MongoClient;
  context: string;
};

export const sync = async (
  beginer: Params,
  destination: Params,
  batchSize = 100,
  timeoutMs = 5000,
) => {
  const queryBeginnerDatabase = await query<User>(beginer);
  const queryDestinationDatabase = await query<User>(destination);

  while (true) {
    const data = await queryBeginnerDatabase
      .find({ isSync: false })
      .limit(batchSize)
      .toArray();
    if (data.length === 0) {
      break;
    }

    try {
      const result = await withTimeout(
        queryDestinationDatabase.insertMany(
          data.map((s) => ({
            ...s,
            isSync: true,
            dateSynced: createDate(new Date()),
          })),
        ),
        timeoutMs,
      );

      if (result.insertedCount > 0) {
        await withTimeout(removeRecords(beginer, data), timeoutMs);
      }
    } catch (ex) {
      throw new Error(`Failed to sync batch: ${(ex as Error).message}`);
    }
  }
};

const removeRecords = async (beginer: Params, users: Array<User>) => {
  const queryBeginnerDatabase = await query<User>(beginer);
  const userIds = users.map((u) => u._id);
  await queryBeginnerDatabase.updateMany({ _id: { $in: userIds } }, { $set: { isSync: true } });
};

const withTimeout = <T>(promise: Promise<T>, ms: number): Promise<T> => {
  return new Promise<T>((resolve, reject) => {
    const timer = setTimeout(() => reject(new Error('Operation timed out')), ms);
    promise
      .then((val) => {
        clearTimeout(timer);
        resolve(val);
      })
      .catch((err) => {
        clearTimeout(timer);
        reject(err);
      });
  });
};
