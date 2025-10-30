import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css'],
  imports: [
    CommonModule,
    RouterModule
  ]
})
export class NavMenuComponent {
  private http = inject(HttpClient);
  private baseUrl = inject<string>('BASE_URL' as any);

  isExpanded = false;
  public buildDateUrl: string;
  public buildDate: string = 'N/A';

  constructor() {
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
