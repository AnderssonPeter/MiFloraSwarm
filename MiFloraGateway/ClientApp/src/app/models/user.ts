export class User {
    id: number;
    username: string;
    password: string;
    firstName: string;
    lastName: string;
    token: string;

    constructor(username?: string, password?:string) {
        this.username = username;
        this.password = password;
    }
}