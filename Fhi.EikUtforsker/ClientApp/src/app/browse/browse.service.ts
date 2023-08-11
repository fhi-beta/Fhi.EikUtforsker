import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { throwError, Observable } from "rxjs";
import { WebDavResource } from "./WebDavResource";

@Injectable({
  providedIn: 'root'
})
export class BrowseService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
  }

  getFolder(uri?: string): Observable<WebDavResource[]> {
    let requestUri = `${this.baseUrl}api/eik/mappe`;
    if (uri) {
      requestUri += `?uri=${uri}`;
    }
    return this.http.get<WebDavResource[]>(requestUri);
  }
}

