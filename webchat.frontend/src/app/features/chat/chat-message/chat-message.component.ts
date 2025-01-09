import { NgClass, NgFor, NgForOf } from '@angular/common';
import { AfterViewChecked, Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { ChatMessage } from '../../../shared/chatMessage';
import { Session } from '../../../shared/session';


@Component({
  selector: 'app-chat-message',
  imports: [
    NgFor,
    NgClass,
    NgForOf
  ],
  templateUrl: './chat-message.component.html',
  styleUrl: './chat-message.component.scss'
})

export class ChatMessageComponent implements AfterViewChecked {
  @Input() userAvatar! : SafeUrl;
  @Input() currentUser! : Session;
  @Input() chatMessages! : ChatMessage[];
  @Input() receivedAt! : string;
  @Input() getAvatarImagePathByAvatarId!: (avatarId: number) => string;

  private scrollToBottom(): void {
    const chatContainer = document.querySelector('.chat-messages');
    if (chatContainer) {
      chatContainer.scrollTop = chatContainer.scrollHeight;
    }
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }
}
