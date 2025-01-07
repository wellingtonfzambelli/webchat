import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AvatarService {

  private _http = inject(HttpClient);
  private _baseUrl = environment.baseUrlAvatar;

  public getAvatarById(id: number) {
    return this._http.get<Blob>(`${this._baseUrl}${id}`, { responseType: 'blob' as 'json' });
  }

  public getRandomNumberByGender(gender: string) {
    if (gender.toLowerCase() === 'male') {
      return Math.floor(Math.random() * 50) + 1;
    } else {
      return Math.floor(Math.random() * 50) + 51;
    }
  }
}