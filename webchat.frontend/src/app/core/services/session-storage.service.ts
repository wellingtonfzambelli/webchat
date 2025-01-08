import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Session } from '../../shared/session';

@Injectable({
  providedIn: 'root'
})

export class SessionStorageService {
  constructor(private router: Router) {}

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

  public logout() : void {
    this.remove();
    this.router.navigate(['/']);
  }

  public remove() : void {
    if(this.verifyExists()) {
      sessionStorage.removeItem(this._sessionNane);
    }
  }
}