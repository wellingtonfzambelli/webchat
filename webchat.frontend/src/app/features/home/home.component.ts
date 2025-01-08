import { Component, inject } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AvatarService } from '../../core/services/avatar.service';
import { SessionStorageService } from '../../core/services/session-storage.service';
import { Session } from '../../shared/session';

@Component({
  selector: 'app-home',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})

export class HomeComponent {
  private _sessionService = inject(SessionStorageService);
  private _avatarService = inject(AvatarService);
  private _formBuilder = inject(FormBuilder);
  private _router = inject(Router);

  public submitted = false;
  
  public loginForm = this._formBuilder.group({
    userName: new FormControl('', [Validators.required]),
    gender: new FormControl('', [Validators.required]),
  });

  constructor() {
    this._sessionService.remove();
  }

  public onSubmit() {
    if (this.loginForm.invalid) {
      this.submitted = true;
    } else {
      const userName = this.loginForm.get('userName')?.value as string;
      const gender = this.loginForm.get('gender')?.value as string;

      const avatarId = this._avatarService.getRandomNumberByGender(gender);
      
      const session = new Session(crypto.randomUUID(), userName, avatarId);
      this._sessionService.create(session);

      this._router.navigateByUrl('/chat');
    }
  }
}