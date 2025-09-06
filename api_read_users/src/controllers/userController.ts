import { Filter, ObjectId } from "mongodb";
import { query } from "../../mongodb-init";
import { User, UserFilter } from "../interfaces/user.interface";
import { USER } from "../shared/context_information";

const getOneUser = async (id: string): Promise<User | null> => {
    const queryUser = await query<User>({ nameCollection: USER.READ_COLLECTION });
    if (!ObjectId.isValid(id)) {
        return null;
    }

    return await queryUser.findOne({ _id: new ObjectId(id) });
}


const getUsers = async (
    { pageNumber, pageSize }: Partial<Pagination>,
    user: UserFilter,
): Promise<User[]> => {
    const queryUser = await query<User>({ nameCollection: USER.READ_COLLECTION });

    const filter: Filter<User> = {};

    if (user.email) {
        filter.email = { $regex: new RegExp(user.email, 'i') };
    }

    if (user.username) {
        filter.username = { $regex: new RegExp(user.username, 'i') };
    }

    let cursor = queryUser.find(filter);

    if (pageSize && pageSize > 0) {
        const currentPage = pageNumber && pageNumber > 0 ? pageNumber : 1;
        cursor = cursor.skip((currentPage - 1) * pageSize).limit(pageSize);
    }

    return cursor.toArray();
}


export { getOneUser, getUsers };
