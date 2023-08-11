import { Component, Input, OnInit, ViewEncapsulation } from "@angular/core";
import { BrowseService } from "./browse.service";
import { FolderEntry } from "./FolderEntry";
import { DekrypterDialogComponent } from './dekrypter-dialog.component';
import { WebDavResource } from "./WebDavResource";
import { MatDialog } from "@angular/material/dialog";

@Component({
  selector: 'app-folder-entry',
  templateUrl: './folder-entry.component.html',
  styleUrls: ['./folder-entry.component.css'],
})
export class FolderEntryComponent implements OnInit {
  @Input() resource: WebDavResource | undefined;
  public folderEntry: FolderEntry | undefined;

  constructor(private browseService: BrowseService, public dialog: MatDialog) { }

  ngOnInit() {
    this.folderEntry = new FolderEntry(this.resource!);
  }

  handleFolderClick(event: Event, folderEntry: FolderEntry): void {
    event.stopPropagation();
    console.log('CLICK ' + folderEntry.resource.uri, folderEntry);
    if (folderEntry.isOpen) {
      folderEntry.isOpen = false;
    } else {
      folderEntry.hasInsertedChildren = false;
      this.browseService.getFolder(folderEntry.resource.uri)
        .subscribe(
          data => {
            folderEntry.children = data.map(resource => new FolderEntry(resource));
            folderEntry.isOpen = true;
            console.log('1 Opens ' + folderEntry.resource.uri, folderEntry);
          },
          error => {
            console.log(error);
          });
    }
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

  decodeURI(uri: string) {
    return decodeURIComponent(uri);
  }
}
