import { ArrayDataSource } from '@angular/cdk/collections';
import { Component } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatDialog } from '@angular/material/dialog';
import { DekrypterDialogComponent } from './dekrypter-dialog.component';
import { EikService, WebDavResource } from '../eik.service';


@Component({
  selector: 'app-browse',
  templateUrl: 'browse.component.html',
  styleUrls: ['browse.component.css']
})
export class FolderExplorerComponent {
  public resources: WebDavResource;
  public treeControl: NestedTreeControl<WebDavResource>;
  public dataSource: ArrayDataSource<WebDavResource>;

  hasChild = (_: number, node: WebDavResource) => node.isCollection;

  constructor(private eikService: EikService, public dialog: MatDialog) {
  }

  ngOnInit(): void {
    var cached = this.eikService.getCachedResourceThree();

    if (cached) {
      this.setResources(cached);
    }
    else {
      this.eikService.getResourceThree()
        .subscribe(resources => {
          this.eikService.setCachedResourceThree(resources);
          this.setResources(resources);
        }, error => console.error(error));
    }
  }

  setResources(resources: WebDavResource): void {
    this.resources = resources;
    this.treeControl = new NestedTreeControl<WebDavResource>(node => node.children);
    this.dataSource = new ArrayDataSource<WebDavResource>([this.resources]);
  }

  dateString(node: WebDavResource) {
    return new Date(node.lastModifiedDate).toLocaleString('nb-NO');
  }

  dekrypter(uri: string) {
    this.dialog.open(DekrypterDialogComponent, {
      data: {
        uri: uri
      }
    });
  }
}
