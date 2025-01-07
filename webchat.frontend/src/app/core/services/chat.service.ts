import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ChatMessage } from '../../shared/chatMessage';

@Injectable({
  providedIn: 'root'
})

export class ChatService {

  private _http = inject(HttpClient);
    private _baseUrl = environment.baseUrlChatApi;
  
    public sendMessage(chatMessage: ChatMessage) {
      return this._http.post(`${this._baseUrl}`, chatMessage);
    }
}
