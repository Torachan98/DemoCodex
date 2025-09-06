import { MongoClient } from 'mongodb';
import { query } from '../../mongodb-connect';
import { User } from '../interfaces/user.interface';
import { createDate } from '../shared/utility';

type Params = {
  nameCollection: string;
  client: MongoClient;
  context: string;
};

export const sync = async (beginer: Params, destination: Params) => {
  const queryBeginnerDatabase = await query<User>(beginer);
  const queryDestinationDatabase = await query<User>(destination);

  const data = await queryBeginnerDatabase.find({ isSync: false }).toArray();
  if (data.length === 0) {
    return;
  }

  try {
    const result = await queryDestinationDatabase.insertMany(
      data.map((s) => ({
        ...s,
        isSync: true,
        dateSynced: createDate(new Date()),
      })),
    );

    if (result.insertedCount > 0) {
      await removeRecords(beginer, data);
    }
  } catch (ex) {
    console.log(ex);
  }
};

const removeRecords = async (beginer: Params, users: Array<User>) => {
  try {
    const queryBeginnerDatabase = await query<User>(beginer);

    const userIds = users.map((u) => u._id);
    await queryBeginnerDatabase.updateMany({ _id: { $in: userIds } }, { $set: { isSync: true } });
  } catch (ex) {
    throw ex;
  }
};
