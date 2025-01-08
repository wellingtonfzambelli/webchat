import { NgClass, NgFor } from '@angular/common';
import { Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { ChatMessage } from '../../../shared/chatMessage';
import { Session } from '../../../shared/session';

@Component({
  selector: 'app-chat-message',
  imports: [
    NgFor,
    NgClass
  ],
  templateUrl: './chat-message.component.html',
  styleUrl: './chat-message.component.scss'
})

export class ChatMessageComponent {
  @Input() userAvatar! : SafeUrl;
  @Input() currentUser! : Session;
  @Input() chatMessages! : ChatMessage[];
  @Input() getAvatarByUserId!: (userId: string) => SafeUrl | null;
}
