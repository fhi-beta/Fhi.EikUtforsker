import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EikService {

  private CachedResourceThreeExpires: Date = new Date();
  private CachedResourceThree: WebDavResource = undefined;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    console.log('EikService.constructor()');
    this.CachedResourceThreeExpires.setHours(-1);
  }

  setCachedResourceThree(three: WebDavResource): void {
    console.log('setCachedResourceThree()');
    this.CachedResourceThree = three;
    this.CachedResourceThreeExpires = new Date();
    this.CachedResourceThreeExpires.setMinutes(this.CachedResourceThreeExpires.getMinutes() + 2);
  }

  getCachedResourceThree(): WebDavResource {
    console.log('getCachedResourceThree()');
    var now = new Date();
    if (this.CachedResourceThreeExpires <= now) {
      console.log('Cleared cache');
      this.CachedResourceThree = undefined;
    }
    return this.CachedResourceThree;
  }

  getResourceThree(): Observable<WebDavResource> {
    console.log('getResourceThree()');
    return this.http.get<WebDavResource>(this.baseUrl + 'api/eik');
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
