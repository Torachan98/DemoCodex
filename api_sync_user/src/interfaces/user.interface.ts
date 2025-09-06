import { ObjectId } from "mongodb";

export interface User {
    _id: ObjectId;
    username: string;
    email: string;
    dateCreated: Date;
    isSync: boolean;
    is_active: boolean;
}

export type UserFilter = Pick<User,"email" | "username">;
export type UserCreate = Pick<User,"email" | "username" | "dateCreated" | "isSync" | "is_active">;
