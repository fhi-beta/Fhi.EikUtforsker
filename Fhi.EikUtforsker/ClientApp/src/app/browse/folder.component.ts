import { Component, OnInit, Input, ViewEncapsulation } from '@angular/core';
import { BrowseService } from './browse.service';
import { FolderEntry } from './FolderEntry';

@Component({
  selector: 'app-folder',
  templateUrl: './folder.component.html',
  styleUrls: ['./folder.component.css'],
})
export class FolderComponent implements OnInit {
  @Input() folderEntry: FolderEntry | undefined;

  constructor(private browseServicce: BrowseService) { }

  ngOnInit(): void {
    if (!this.folderEntry!.children) {
      this.folderEntry!.children = [];
    }
  }

  handleFolderClick(folderEntry: any): void {
    if (folderEntry.isOpen) {
      console.log('IsOpen ' + folderEntry.resource.uri, folderEntry);
      folderEntry.isOpen = false;
    } else {
      this.browseServicce.getFolder(folderEntry.uri)
        .subscribe(
          data => {
            folderEntry.children = data;
            folderEntry.isOpen = true;
            console.log('2 Opens ' + folderEntry.resource.uri, folderEntry);
          },
          error => {
            console.log(error);
          });
    }
  }
}
