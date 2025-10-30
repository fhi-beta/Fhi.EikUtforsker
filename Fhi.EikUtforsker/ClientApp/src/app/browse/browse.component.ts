import { Component, inject } from '@angular/core';
import { BrowseService } from './browse.service';
import { WebDavResource } from './WebDavResource';
import { FolderEntryComponent } from './folder-entry.component';
import { MatListModule } from '@angular/material/list';



@Component({
  selector: 'app-browse',
  templateUrl: 'browse.component.html',
  styleUrls: ['browse.component.css'],
  imports: [
    FolderEntryComponent,
    MatListModule
  ]
})

export class FolderExplorerComponent {
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
        this.resources = resources;
        this.erFerdigLastet = true;
      }, error => console.error(error));
  }
}
