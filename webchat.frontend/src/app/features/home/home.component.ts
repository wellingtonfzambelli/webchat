import { Component, inject } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-home',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})

export class HomeComponent {
  private _formBuilder = inject(FormBuilder);
  
  public submitted = false;
  
  public loginForm = this._formBuilder.group({
    userName: new FormControl('', [Validators.required]),
    gender: new FormControl('', [Validators.required]),
  });

  public onSubmit() {
    if (this.loginForm.invalid) {
      this.submitted = true;
    }else{
      this.submitted = false;
      //this.loginForm.reset();
    }
  }
}