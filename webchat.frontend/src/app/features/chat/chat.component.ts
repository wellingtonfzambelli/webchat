import { NgFor, NgIf, TitleCasePipe } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ChatService } from '../../core/services/chat.service';
import { SessionStorageService } from '../../core/services/session-storage.service';
import { SignalrService } from '../../core/services/signalr.service';
import { ChatMessage } from '../../shared/chatMessage';
import { Session } from '../../shared/session';
import { ChatHeadComponent } from "./chat-head/chat-head.component";
import { ChatMessageComponent } from "./chat-message/chat-message.component";

@Component({
  selector: 'app-chat',
  imports: [
    ReactiveFormsModule,
    ChatHeadComponent,
    ChatMessageComponent,
    TitleCasePipe,
    NgFor,
    NgIf    
],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})

export class ChatComponent implements OnInit, OnDestroy {  
  private _chatService = inject(ChatService);
  private _formBuilder = inject(FormBuilder);

  public route = inject(Router);
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
    this.initializeSignalR();
    
    this.currentUser = new Session(
      this.sessionService.get()?.userId as string, 
      this.sessionService.get()?.userName as string, 
      this.sessionService.get()?.avatarId as number
    );
  }

  public getAvatarImagePathByAvatarId(avatarId: number) : string {
    return `./images/avatars/AV${avatarId}.png`;
  }  

  public logout() : void {
    this.sessionService.remove();
    this.signalrService.stopHubConnection();
  }

  private initializeSignalR() : void {
    this.signalrService.buildConnection(
      this.sessionService.get()!.userId,
      this.sessionService.get()!.userName,
      this.sessionService.get()!.avatarId
    )
    this.signalrService.startConnection();
    this.signalrService.listenMessages();
    this.signalrService.listenForOnlineUsers();
  }

  ngOnDestroy(): void {
    this.signalrService.startConnection();
  }

  getCurrentDate(): string {
    const now = new Date();
    
    const hours = now.getHours();
    const minutes = now.getMinutes();
    const day = now.getDate();
    const month = now.getMonth() + 1;
    const year = now.getFullYear();
    
    const ampm = hours >= 12 ? 'pm' : 'am';
    
    const hour12 = hours % 12 || 12; 
  
    const formattedMinutes = minutes < 10 ? '0' + minutes : minutes;
  
    // return `${day}/${month}/${year} ${hour12}:${formattedMinutes} ${ampm}`;
    return `${hour12}:${formattedMinutes} ${ampm}`;
  }
}