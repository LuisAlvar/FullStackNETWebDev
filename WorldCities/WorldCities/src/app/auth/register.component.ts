import { Component, OnInit } from '@angular/core';
import { BaseFormComponent } from '../base-form.component';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterResult } from './register-result';
import { RegisterRequest } from './register-request';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent extends BaseFormComponent implements OnInit {

  registerResult?: RegisterResult;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      username: new FormControl('', Validators.required),
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required)
    });
  }

  onSubmit() {
    var registerRequest = <RegisterRequest>{};
    registerRequest.username = this.form.controls['username'].value;
    registerRequest.email = this.form.controls['email'].value;
    registerRequest.password = this.form.controls['password'].value;

    if (isNullOrEmpty(registerRequest.username)
      || isNullOrEmpty(registerRequest.email)
      || isNullOrEmpty(registerRequest.password)
    ) {
      return;
    }

    this.authService
      .register(registerRequest)
      .subscribe(result => {
        this.registerResult = result;
      }, error => {
        console.log(error.error);
        this.registerResult = error.error;
      });

    this.form.patchValue({
      password: ''
    });

  }


}

function isNullOrEmpty(str: string | null | undefined): boolean {
  return (str ?? '').trim().length === 0;
}
