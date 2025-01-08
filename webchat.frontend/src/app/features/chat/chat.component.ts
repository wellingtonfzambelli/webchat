import { DatePipe, NgClass, NgFor, NgIf, TitleCasePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { AvatarService } from '../../core/services/avatar.service';
import { ChatService } from '../../core/services/chat.service';
import { SessionStorageService } from '../../core/services/session-storage.service';
import { SignalrService } from '../../core/services/signalr.service';
import { ChatMessage } from '../../shared/chatMessage';

@Component({
  selector: 'app-chat',
  imports: [
    TitleCasePipe,
    DatePipe,
    ReactiveFormsModule,
    NgFor,
    NgIf,
    NgClass
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})

export class ChatComponent implements OnInit {
  private _avatarService = inject(AvatarService);
  private _chatService = inject(ChatService);
  private _sanitizer = inject(DomSanitizer);
  private _formBuilder = inject(FormBuilder);
  
  public signalrService = inject(SignalrService);
  public sessionService = inject(SessionStorageService);
  public avatarUrl: SafeUrl | null = null;
  public chatMessages: ChatMessage[] = [];

  public chatForm = this._formBuilder.group({
      message: new FormControl('', [Validators.required])      
  });

  public onSubmit() {
      if (this.chatForm.invalid) {
        
      } else {
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
    this.fetchAvatar(this.sessionService.get()?.avatarId as number);
    
    this.signalrService.createHubConnection();
    
    this.signalrService.hubConnection?.on("ChatHub", (hubMessage: string) => {

      const hubMessageJson = JSON.parse(hubMessage);

      const chatMessage = new ChatMessage(
        hubMessageJson.UserId, 
        hubMessageJson.UserName,
        hubMessageJson.AvatarId,
        hubMessageJson.Message
      );

      this.chatMessages.push(chatMessage);
    })
  }

  fetchAvatar(id: number) {
    this._avatarService.getAvatarById(id).subscribe({
      next: (blob: Blob) => {
        const objectUrl = URL.createObjectURL(blob);
        this.avatarUrl = this._sanitizer.bypassSecurityTrustUrl(objectUrl);
      },
      error: (err) => {
        console.error('Error fetching avatar:', err);
      },
    });
  }
}