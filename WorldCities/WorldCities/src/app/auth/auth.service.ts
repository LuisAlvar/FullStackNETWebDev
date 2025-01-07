import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from './../../envrionments/environment';
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';


@Injectable({
  providedIn: 'root',
})
export class AuthService {

  public tokenKey: string = "token";

  constructor(protected http: HttpClient) { }


  isAuthenticated(): boolean {
    return this.getToken() != null;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  login(item: LoginRequest): Observable<LoginResult> {
    var url = environment.baseURL + "api/Account/Login";
    return this.http.post<LoginResult>(url, item);
  }

}
