import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

export class AvatarService {
  public getRandomNumberByGender(gender: string) {
    if (gender.toLowerCase() === 'male') {
      return Math.floor(Math.random() * 50) + 1;
    } else {
      return Math.floor(Math.random() * 50) + 51;
    }
  }
}