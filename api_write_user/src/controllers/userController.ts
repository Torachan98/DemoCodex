import { ObjectId } from "mongodb";
import { query } from "../../mongodb-init";
import { User, UserCreate } from "../interfaces/user.interface";
import { USER } from "../shared/context_information";


const createUser = async (user: UserCreate): Promise<User> => {
    const queryUser = await query<User>({ nameCollection: USER.WRITE_COLLECTION });
    // if(user._id !== null) {
    //     return {} as User;
    // }

    const payload = {
        ...user, 
        _id: new ObjectId(), 
        isSync: false, 
        is_active: true,
        dateCreated:createDate(new Date()) 
    };

    const objectId = await queryUser.insertOne(payload);

    const userCreated: User = {
        ...payload,
        _id: objectId.insertedId
    }

    return userCreated;
}


const updateUser = async (id: string, user: Partial<User>): Promise<User | null> => {
    const queryUser = await query<User>({ nameCollection: USER.WRITE_COLLECTION });
    if (!ObjectId.isValid(id)) {
        return null;
    }

    const updateDoc: Partial<User> = {
        ...user,
        isSync: false,
        dateCreated: createDate(new Date()),
    };

    await queryUser.updateOne({ _id: new ObjectId(id) }, { $set: updateDoc });
    return queryUser.findOne({ _id: new ObjectId(id) });
}


const deleteUser = async (id: string): Promise<boolean> => {
    const queryUser = await query<User>({ nameCollection: USER.WRITE_COLLECTION });
    if (!ObjectId.isValid(id)) {
        return false;
    }

    const remove = await queryUser.deleteOne({ _id: new ObjectId(id) });
    return remove.deletedCount > 0;
}

const createDate = (date: Date = new Date()): Date => {
    return new Date(
      Date.UTC(
        date.getUTCFullYear(),
        date.getUTCMonth(),
        date.getUTCDay(),
        date.getUTCHours(),
        date.getUTCMinutes(),
        date.getUTCSeconds(),
        date.getUTCMilliseconds()
      )
    );
}


export { createUser, updateUser, deleteUser};
