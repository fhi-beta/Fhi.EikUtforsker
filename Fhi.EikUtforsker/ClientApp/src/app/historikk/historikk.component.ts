import { ArrayDataSource } from '@angular/cdk/collections';
import { Component, ViewChild, AfterViewInit, ElementRef } from '@angular/core';
import { DekrypterDialogComponent } from '../browse/dekrypter-dialog.component';
import { BrowseService } from '../browse/browse.service';
import { WebDavResource } from '../browse/WebDavResource';

@Component({
  selector: 'app-historikk',
  templateUrl: './historikk.component.html'
})
export class HistorikkComponent {
  public erFerdigLastet: boolean = false;
  public resources: WebDavResource[] = [];

  constructor(private browseService: BrowseService) {
  }

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
