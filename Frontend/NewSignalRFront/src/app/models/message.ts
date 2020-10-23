import { Role } from "./role";

export class MessageModel 
    implements Role 
{
    name: string;
    status: number;
    role: string;

    constructor(Name: string, Status: number, Role: string){
        this.name = Name;
        this.status = Status;
        this.role = Role;
    }
}