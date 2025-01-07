import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class SignalrService {

  public hubUrl = environment.signalHubUrl;
  public hubConnection?: HubConnection;
  public orderSignal = signal<string | null>(null);

  public createHubConnection() {
    debugger
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true // includes credentials (cookies or authentication)
      })
      .withAutomaticReconnect()
      .build();

      this.hubConnection.start()
        .catch(error => console.log(error));

      this.hubConnection.on("ChatHub", (order: string) => {
        //this.orderSignal.set(order);
        alert('signal: ' + order);
      })
  }

  public stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch(error => console.log(error))
    }
  }
}
