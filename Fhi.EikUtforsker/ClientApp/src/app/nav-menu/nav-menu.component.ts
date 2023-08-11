import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  isExpanded = false;
  public buildDateUrl: string;
  public buildDate: string = 'N/A';

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.buildDateUrl = this.baseUrl + 'api/builddate';
  }

  ngOnInit(): void {
    this.http.get<BuildDateResponse>(this.buildDateUrl).subscribe(result => {
      console.log(result);
      this.buildDate = result.buildDate.substring(0, 10);
    }, error => console.error(error));
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}

class BuildDateResponse {
  buildDate: string;

  constructor(buildDate: string) {
    this.buildDate = buildDate;
  }
}
