export class Session {

    constructor(userId: string, userName: string, avatarId: number){
        this.userId = userId;
        this.userName = userName;
        this.avatarId = avatarId;    
    }

    userId: string;
    userName: string;
    avatarId: number;
}