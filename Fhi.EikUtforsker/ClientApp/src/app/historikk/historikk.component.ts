import { Component, OnInit, inject } from '@angular/core';
import { BrowseService } from '../browse/browse.service';
import { WebDavResource } from '../browse/WebDavResource';
import { HistorikkEntryComponent } from './historikk-entry.component';
import { MatListModule } from '@angular/material/list';

@Component({
  selector: 'app-historikk',
  templateUrl: './historikk.component.html',
  imports: [
    HistorikkEntryComponent,
    MatListModule
  ]
})
export class HistorikkComponent implements OnInit {
  private browseService = inject(BrowseService);

  public erFerdigLastet: boolean = false;
  public resources: WebDavResource[] = [];

  ngOnInit(): void {
    this.lastResources();
  }

  lastResources() {
    console.log('Laster...');
    this.browseService.getFolder()
      .subscribe(resources => {
        console.log(resources);

        this.resources = resources;

        this.resources = this.resources.sort((a, b) => {
          const dateA = Date.parse(`${a.lastModifiedDate}`);
          const dateB = Date.parse(`${b.lastModifiedDate}`);
          return dateB - dateA;
        });
        this.erFerdigLastet = true;
      }, error => console.error(error));
  }


}
