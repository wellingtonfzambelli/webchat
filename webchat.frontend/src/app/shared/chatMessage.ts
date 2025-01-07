export class ChatMessage {

    constructor(userId: string, userName: string, avatarId: number, message: string){
        this.userId = userId;
        this.userName = userName;
        this.avatarId = avatarId;    
        this.message = message;
    }

    userId: string;
    userName: string;
    avatarId: number;
    message: string;
}