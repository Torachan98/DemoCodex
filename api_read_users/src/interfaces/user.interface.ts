import { ObjectId } from "mongodb";

export interface User {
    _id: ObjectId;
    username: string;
    email: string;
    is_active: boolean;
}

export type UserFilter = Pick<User,"email" | "username">
