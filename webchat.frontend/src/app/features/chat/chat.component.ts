import { NgClass, NgFor } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AvatarService } from '../../core/services/avatar.service';
import { ChatService } from '../../core/services/chat.service';
import { SessionStorageService } from '../../core/services/session-storage.service';
import { SignalrService } from '../../core/services/signalr.service';
import { AvatarUser } from '../../shared/avatarUser';
import { ChatMessage } from '../../shared/chatMessage';
import { Session } from '../../shared/session';
import { ChatHeadComponent } from "./chat-head/chat-head.component";
import { ChatMessageComponent } from "./chat-message/chat-message.component";

@Component({
  selector: 'app-chat',
  imports: [
    ReactiveFormsModule,
    ChatHeadComponent,
    ChatHeadComponent,
    ChatMessageComponent,
    NgFor,
    NgClass
],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})

export class ChatComponent implements OnInit, OnDestroy {
  
  private _avatarService = inject(AvatarService);
  private _chatService = inject(ChatService);
  private _sanitizer = inject(DomSanitizer);
  private _formBuilder = inject(FormBuilder);
  private _userAvatars: AvatarUser[] = [];

  public signalrService = inject(SignalrService);
  public sessionService = inject(SessionStorageService);
  public currentUser!: Session;

  public chatForm = this._formBuilder.group({
      message: new FormControl('', [Validators.required])      
  });



  public onSubmit() {
    if (!this.chatForm.invalid) {
      const message = this.chatForm.get('message')?.value as string;  
      
      const chatMessage = new ChatMessage(
        this.sessionService.get()?.userId as string,
        this.sessionService.get()?.userName as string,
        this.sessionService.get()?.avatarId as number,          
        message
      )

      this._chatService.sendMessage(chatMessage).subscribe({
        next: () => {
          this.chatForm.reset();
          console.log('Message sent successfully!');
        },
        error: (err) => {
          console.error('Failed to send message', err);
        }
      });
    }
  }

  ngOnInit(): void {

    this.signalrService.startConnection();
    this.signalrService.listenMessages();
    
    this.currentUser = new Session(
      this.sessionService.get()?.userId as string, 
      this.sessionService.get()?.userName as string, 
      this.sessionService.get()?.avatarId as number
    );

    this.bindAvatarUsers(this.currentUser.userId, this.currentUser.avatarId);
  }

  public bindAvatarUsers(userId: string, avatarId: number) {
    this._avatarService.getAvatarById(avatarId).subscribe({
      next: (blob: Blob) => {
        const objectUrl = URL.createObjectURL(blob);
        const trustUrl = this._sanitizer.bypassSecurityTrustUrl(objectUrl);

        this._userAvatars = [...this._userAvatars, new AvatarUser(userId, trustUrl)];
      },
      error: (err) => {
        console.error('Error fetching avatar:', err);
      },
    });
  }

  public getAvatarByUserId(userId?: string): SafeUrl | null{
    const userAvatar = this._userAvatars.find(s => s.userId === userId);
    return userAvatar ? userAvatar.userAvatar : null;
  }

  ngOnDestroy(): void {
    this.signalrService.startConnection();
  }
}