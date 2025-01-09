import { TitleCasePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { HubConnectionState } from '@microsoft/signalr';
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
  @Input() userAvatar! : string;
  @Input() currentUser! : Session;
  @Input() connecitonState! : HubConnectionState;
}