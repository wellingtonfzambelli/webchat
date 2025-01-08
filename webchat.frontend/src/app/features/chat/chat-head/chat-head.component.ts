import { TitleCasePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { SafeUrl } from '@angular/platform-browser';
import { Session } from '../../../shared/session';

@Component({
  selector: 'app-chat-head',
  imports: [
    TitleCasePipe
  ],
  templateUrl: './chat-head.component.html',
  styleUrl: './chat-head.component.scss'
})

export class ChatHeadComponent {
  @Input() userAvatar : SafeUrl | null = null;
  @Input() currentUser! : Session;
}