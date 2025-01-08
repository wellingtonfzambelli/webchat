import { SafeUrl } from "@angular/platform-browser";

export class AvatarUser {

    constructor(userId: string, userAvatar: SafeUrl){
        this.userId = userId;
        this.userAvatar = userAvatar;
    }

    userId: string;
    userAvatar: SafeUrl
}