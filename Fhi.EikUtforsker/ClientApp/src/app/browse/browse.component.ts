import { ArrayDataSource } from '@angular/cdk/collections';
import { Component, ViewChild, AfterViewInit, ElementRef } from '@angular/core';
import { DekrypterDialogComponent } from './dekrypter-dialog.component';
import { BrowseService } from './browse.service';
import { WebDavResource } from './WebDavResource';


@Component({
  selector: 'app-browse',
  templateUrl: 'browse.component.html',
  styleUrls: ['browse.component.css']
})

export class FolderExplorerComponent {
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
        this.resources = resources;
        this.erFerdigLastet = true;
      }, error => console.error(error));
  }
}
