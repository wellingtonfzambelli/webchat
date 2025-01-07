export class Session {
    
    constructor(userName: string, avatarId: number){
        this.userName = userName;
        this.avatarId = avatarId;    
    }

    userName: string;
    avatarId: number;
}