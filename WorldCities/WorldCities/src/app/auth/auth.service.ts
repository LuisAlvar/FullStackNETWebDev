import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, tap } from 'rxjs';

import { environment } from './../../envrionments/environment';
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';
import { RegisterRequest } from './register-request';
import { RegisterResult } from './register-result';

import { jwtDecode } from 'jwt-decode';
import { isNullOrEmpty } from '../utils/Strings';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root',
})
export class AuthService {

  private strKeyAccessToken: string = "atoken";
  private strKeyExpAccessToken: string = "aexptoken";

  private _authStatus = new Subject<boolean>();
  public authStatus = this._authStatus.asObservable();

  public email?: string

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  isAuthenticated(): boolean {
    return this.getAccessToken() != null;
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.strKeyAccessToken);
  }

  getAccessTokenExp(): Date | null {
    var strDateTime = localStorage.getItem(this.strKeyExpAccessToken) as string ?? "";
    if (isNullOrEmpty(strDateTime)) return null;
    var result = new Date(strDateTime);
    return result;
  }

  refreshToken(): Observable<any> {
    return this.http.post("api/Account/RefreshTokens", { email: this.email })
      .pipe(tap((response: any) => {
        this.processAccessToken(response.Token);
      }));
  }

  init(): void {
    if (this.isAuthenticated()) this.setAuthStatus(true);
  }

  login(item: LoginRequest): Observable<LoginResult> {
    var url = environment.baseURL + "api/Account/Login";
    return this.http
      .post<LoginResult>(url, item)
      .pipe(tap(loginResult => {
        if (loginResult.success && loginResult.token) {
          this.processAccessToken(loginResult.token);
          this.setAuthStatus(true);
          this.email = item.email;
        }
      }));
  }

  register(item: RegisterRequest): Observable<RegisterResult> {
    var url = environment.baseURL + "api/Account/Register";
    return this.http.post<RegisterResult>(url, item);
  }

  logout() {
    localStorage.removeItem(this.strKeyAccessToken);
    localStorage.removeItem(this.strKeyExpAccessToken);
    this.cookieService.delete('refreshToken', '/', 'localhost');
    this.setAuthStatus(false);
  }

  private setAuthStatus(isAuthenticated: boolean): void {
    this._authStatus.next(isAuthenticated);
  }

  private processAccessToken(tokens: string): void
  {
    // Set the Access Token to local store
    const currentTime = new Date();
    localStorage.setItem(this.strKeyAccessToken, tokens);
    // Add the expired time to the local store as while 
    const decodedAccessToken = this.DecodeAnyToken(tokens);
    if (decodedAccessToken) {
      const expTime = decodedAccessToken.exp;
      const expirDateTime = new Date(expTime * 1000);
      console.log("This is the expired date for access token: " + expirDateTime.toUTCString());
      localStorage.setItem(this.strKeyExpAccessToken, expirDateTime.toUTCString())
    }
  }

  private DecodeAnyToken(token: string): any {
    try {
      const decodedToken = jwtDecode(token);
      return decodedToken;
    } catch (e) {
      console.error('Invalid token', e);
      return null;
    }
  }
}
