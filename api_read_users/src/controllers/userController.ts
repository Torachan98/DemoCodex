import { ObjectId } from "mongodb";
import { query } from "../../mongodb-init";
import { User, UserFilter } from "../interfaces/user.interface";
import { USER } from "../shared/context_information";

const getOneUser = async (id: string): Promise<User> => {
    const queryUser = await query<User>({ nameCollection: USER.READ_COLLECTION });
    if(id === "") {
        return {} as User;
    }
    
    return await queryUser.findOne({ _id: new ObjectId(id) });
}


const getUsers = async ({pageNumber, pageSize}: Partial<Pagination>, user: UserFilter): Promise<User[]> => {
    const queryUser = await query<User>({ nameCollection: USER.READ_COLLECTION });

    const filter: Partial<User> = {};

    if(user.email!){
        filter.email = user.email;
    }

    if(user.username!){
        filter.username = user.username;
    }

    return queryUser
    .find(filter)
    .skip((pageNumber - 1) * pageSize)
    .limit(pageSize)
    .toArray();
}


export { getOneUser, getUsers };
