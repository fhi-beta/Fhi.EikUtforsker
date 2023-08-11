import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'dekrypter-dialog',
  templateUrl: 'dekrypter-dialog.component.html'
})
export class DekrypterDialogComponent {
  public uri: string;
  public analyse: Dekrypteringsanalyse | undefined;

  public krypterUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, @Inject(MAT_DIALOG_DATA) data:any) {
    this.uri = data.uri;
    this.krypterUrl = this.baseUrl + 'api/eik/dekrypter';
  }

  ngOnInit(): void {
    this.http.get<Dekrypteringsanalyse>(this.baseUrl + 'api/eik/analyse?uri=' + this.uri).subscribe(result => {
      console.log(result);
      this.analyse = result;
    }, error => console.error(error));

    this.http.get<Dekrypteringsanalyse>("");
  }
}

interface Dekrypteringsanalyse {
  erGyldigJson: boolean;
  rotElement: string;
  skjemanavn: string;
  erSkjemavalidert: boolean;
  skjemavalideringsfeil: string;
  kanDekrypteres: boolean;
  dekrypteringsfeil: string;
  thumbprint: string;
  antallBytesDekryptert: number;
  dekrypteringUrl: string;
  dekryptertFilnavn: string;
  skjemavalideringsfeilDekryptert: Array<string>;
}
