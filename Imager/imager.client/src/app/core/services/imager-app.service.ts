import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ImagerAppService {

  private apiUrl = 'https://localhost:7196/api/v1/ImagerApp';

  constructor(private http: HttpClient) { }

  uploadImage(formData: FormData): Observable<HttpEvent<any>> {
    const req = new HttpRequest('POST', `${this.apiUrl}/upload`, formData, {
      reportProgress: true
    });

    return this.http.request(req);
  }
}
