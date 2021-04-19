import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { DekrypterDialogComponent } from '../browse/dekrypter-dialog.component';

@Component({
  selector: 'app-historikk',
  templateUrl: './historikk.component.html'
})
export class HistorikkComponent {
  public historikk: WebDavFile[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public dialog: MatDialog) {
    http.get<WebDavFile[]>(baseUrl + 'api/eik/historikk').subscribe(result => {
      this.historikk = result;
    }, error => console.error(error));
  }

  dateString(file: WebDavFile) {
    return new Date(file.lastModifiedDate).toLocaleString('nb-NO');
  }

  dekrypter(uri: string) {
    this.dialog.open(DekrypterDialogComponent, {
      data: {
        uri: uri
      }
    });
  }
}

interface WebDavFile {
  uri: string;
  relUri: string;
  eTag: string;
  lastModifiedDate: Date;
  contentLength: number;
}
