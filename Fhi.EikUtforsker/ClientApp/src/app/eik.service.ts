import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EikService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  getResourceThree(antallDager: number): Observable<WebDavResource> {
    return this.http.get<WebDavResource>(this.baseUrl + 'api/eik?antallDager=' + antallDager);
  }
}

export interface WebDavResource {
  uri: string;
  relUri: string;
  eTag: string;
  lastModifiedDate: Date;
  contentLength: number;
  isCollection: boolean;
  children: WebDavResource[]
}
