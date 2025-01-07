export class Session {

    constructor(userName: string, avatarId: number){
        this.userId = crypto.randomUUID();
        this.userName = userName;
        this.avatarId = avatarId;    
    }

    userId: string;
    userName: string;
    avatarId: number;
}