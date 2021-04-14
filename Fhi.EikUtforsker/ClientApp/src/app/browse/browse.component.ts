import { ArrayDataSource } from '@angular/cdk/collections';
import { Component, Inject } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { DekrypterDialogComponent } from './dekrypter-dialog.component';


@Component({
  selector: 'app-browse',
  templateUrl: 'browse.component.html',
  styleUrls: ['browse.component.css']
})
export class FolderExplorerComponent {
  public resources: WebDavResource;
  public treeControl: NestedTreeControl<WebDavResource>;
  public dataSource: ArrayDataSource<WebDavResource>;

  //hasChild = (_: number, node: WebDavResource) => !!node.children && node.children.length > 0;
  hasChild = (_: number, node: WebDavResource) => node.isCollection;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, public dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.http.get<WebDavResource>(this.baseUrl + 'api/eik').subscribe(result => {
      this.resources = result;
      this.treeControl = new NestedTreeControl<WebDavResource>(node => node.children);
      this.dataSource = new ArrayDataSource<WebDavResource>([this.resources]);
    }, error => console.error(error));
  }

  dateString(node: WebDavResource) {
    return new Date(node.lastModifiedDate).toLocaleString('nb-NO');
  }

  dekrypter(uri: string) {
    this.dialog.open(DekrypterDialogComponent, {
      data: {
        uri: uri
      }
    });  }

}

interface WebDavResource {
  uri: string;
  relUri: string;
  eTag: string;
  lastModifiedDate: Date;
  contentLength: number;
  isCollection: boolean;
  children: WebDavResource[]
}
