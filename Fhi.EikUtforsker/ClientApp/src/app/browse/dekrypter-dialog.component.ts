import { Component, OnInit, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'dekrypter-dialog',
  templateUrl: 'dekrypter-dialog.component.html',
  imports: [
    MatDialogModule
  ]
})
export class DekrypterDialogComponent implements OnInit {
  private http = inject(HttpClient);
  private baseUrl = inject<string>('BASE_URL' as any);

  public uri: string;
  public analyse: Dekrypteringsanalyse | undefined;

  public krypterUrl: string;

  constructor() {
    const data = inject(MAT_DIALOG_DATA);

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
