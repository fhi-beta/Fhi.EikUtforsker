import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable } from "rxjs";
import { WebDavResource } from "./WebDavResource";

@Injectable({
  providedIn: 'root'
})
export class BrowseService {
  private http = inject(HttpClient);
  private baseUrl = inject<string>('BASE_URL' as any);


  getFolder(uri?: string): Observable<WebDavResource[]> {
    let requestUri = `${this.baseUrl}api/eik/mappe`;
    if (uri) {
      requestUri += `?uri=${uri}`;
    }
    return this.http.get<WebDavResource[]>(requestUri);
  }
}

