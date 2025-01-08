import { Injectable } from '@angular/core';
import { Session } from '../../shared/session';

@Injectable({
  providedIn: 'root'
})

export class SessionStorageService {
  private _sessionNane : string = "webchame-session";

  public verifyExists() : boolean {
    const existingValue = sessionStorage.getItem(this._sessionNane);
    
    if(existingValue)
      return true;
    
    return false;
  }

  public get() : Session | null {
    const storedData = sessionStorage.getItem(this._sessionNane);

    if (storedData) {
        const jsonData = JSON.parse(storedData);
        return new Session(jsonData.userId, jsonData.userName, jsonData.avatarId);
    }

    return null;
  }

  public create(value: Session) : void {
    sessionStorage.setItem(this._sessionNane, JSON.stringify(value));
  }

  public remove() : void {
    if(this.verifyExists()) {
      sessionStorage.removeItem(this._sessionNane);
    }
  }
}