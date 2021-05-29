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
  public antallDager: number = 7;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, public dialog: MatDialog) {
  }

  ngOnInit(): void {
    this.lastHistorikk();
  }

  lastHistorikk() {
    this.http.get<WebDavFile[]>(this.baseUrl + 'api/eik/historikk?antallDager=' + this.antallDager).subscribe(result => {
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
