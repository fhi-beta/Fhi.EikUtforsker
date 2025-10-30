import { Component, OnInit, inject, input } from "@angular/core";
import { BrowseService } from "../browse/browse.service";
import { FolderEntry } from "../browse/FolderEntry";
import { DekrypterDialogComponent } from '../browse/dekrypter-dialog.component';
import { WebDavResource } from "../browse/WebDavResource";
import { MatDialog } from "@angular/material/dialog";
import { MatListModule } from "@angular/material/list";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatIconModule } from "@angular/material/icon";

@Component({
  selector: 'app-historikk-entry',
  templateUrl: './historikk-entry.component.html',
  imports: [
    MatListModule,
    MatTooltipModule,
    MatExpansionModule,
    MatIconModule
  ]
})
export class HistorikkEntryComponent implements OnInit {
  private browseService = inject(BrowseService);
  dialog = inject(MatDialog);

  readonly resource = input<WebDavResource>();
  public folderEntry: FolderEntry | undefined;

  ngOnInit() {
    this.folderEntry = new FolderEntry(this.resource()!);
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

            folderEntry.children = folderEntry.children.sort((a, b) => {
              const dateA = Date.parse(`${a.resource.lastModifiedDate}`);
              const dateB = Date.parse(`${b.resource.lastModifiedDate}`);
              return dateB - dateA;
            });


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
