import { Injectable } from '@angular/core';
import { HttpClient, HttpEvent, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ImagerAppService {

  private apiUrl = environment.apiUrl + '/ImagerApp';

  constructor(private http: HttpClient) { }

  uploadImage(formData: FormData): Observable<HttpEvent<any>> {
    console.log(this.apiUrl);
    const req = new HttpRequest(
      'POST',
      `${this.apiUrl}/upload`,
      formData,
      { reportProgress: true }
    );

    return this.http.request(req);
  }

  getImageUrl(id: number): string {
    return `${this.apiUrl}/image/${id}`;
  }

}
