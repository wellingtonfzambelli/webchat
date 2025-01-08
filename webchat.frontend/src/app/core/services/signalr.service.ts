import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { ChatMessage } from '../../shared/chatMessage';

@Injectable({
  providedIn: 'root'
})

export class SignalrService {

  public hubUrl = environment.signalHubUrl;
  public hubConnection?: HubConnection;
  public chatMessageSignal = signal<ChatMessage | null>(null);

  public createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true // includes credentials (cookies or authentication)
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start()
        .catch(error => console.log(error));


  }

  public stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error))
    }
  }
}
