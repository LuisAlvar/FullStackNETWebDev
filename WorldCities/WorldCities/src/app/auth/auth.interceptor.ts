import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { catchError, Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';
import { isNullOrEmpty } from '../utils/Strings';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {

  constructor(
    private authService: AuthService,
    private router: Router)
  { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>>
  {
    // get the auth token
    var token = this.getProperToken();

    // if the token is present, clone the request
    // replacing the original headers with the authorization
    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    // send the request to the next handler
    return next.handle(req)
      .pipe(catchError((error) => {
        // Perform Logout on 401 - Unauthorized HTTP response errors
        if (error instanceof HttpErrorResponse && error.status === 401 && !this.authService.isAuthenticated()) {
          this.authService.logout();
          this.router.navigate(['login']);
        }
        return throwError(error);
      }));
  }

  private getProperToken(): string | null
  {
    // Use this code block to determine what token to use and whether we need to refresh the tokens.
    const curDateTime = new Date();
    var IsUserAuth = this.authService.isAuthenticated();
    var tokenToUse;
    if (!IsUserAuth) return "";
    var dateOfExpirationOnAccessToken = this.authService.getAccessTokenExp() as Date;
    var IsAcccessTokenExpired = curDateTime >= dateOfExpirationOnAccessToken;
    if (!IsAcccessTokenExpired) return this.authService.getAccessToken() as string;
    this.authService.refreshToken();
    return this.authService.getAccessToken();
  }

}
