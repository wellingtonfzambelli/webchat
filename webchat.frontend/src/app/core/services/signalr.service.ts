import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { ChatMessage } from '../../shared/chatMessage';

@Injectable({
  providedIn: 'root'
})

export class SignalrService {

  private _hubUrl = environment.signalHubUrl;
  private _hubConnection: HubConnection;
  
  private _onlineUsers: string[] = [];
  private _messages: ChatMessage[] = [];

  constructor(){
    this._hubConnection = new HubConnectionBuilder()
    .withUrl(this._hubUrl, {
      withCredentials: true // includes credentials (cookies or authentication)
    })
    .withAutomaticReconnect()
    .build();
  }

  public getMessages() : ChatMessage[] {
    return this._messages;
  }

  public getOnlineUsers() : string[] {
    return this._onlineUsers;
  }

  public listenForOnlineUsers(): void {
    this._hubConnection.on('UpdateOnlineUsers', (users: string[]) => {
      this._onlineUsers = users;
    });
  }

  public listenMessages() : ChatMessage {
    let chatMessage: ChatMessage | null = null;

    this._hubConnection.on("ChatHub", (hubMessage: string) => {
      const hubMessageJson = JSON.parse(hubMessage);

      chatMessage = new ChatMessage(
        hubMessageJson.UserId, 
        hubMessageJson.UserName,
        hubMessageJson.AvatarId,
        hubMessageJson.Message
      );

      this._messages = [...this._messages, chatMessage];
    });

    return chatMessage!; 
  }

  public startConnection(): void {
    if (this._hubConnection.state === HubConnectionState.Disconnected) {
      this._hubConnection
        .start()
        .then(() => console.log('Connection started'))
        .catch(err => console.error('Error while starting connection:', err));
    }
  }

  public stopHubConnection() {
    if (this._hubConnection.state === HubConnectionState.Connected) {
      this._hubConnection
        .stop()
        .then(() => console.log('Connection closed'))
        .catch(error => console.log("Error while closing connection"));
    }
  }

  public getConnectionState(): HubConnectionState {
    return this._hubConnection.state;
  }
}