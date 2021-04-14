import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HistorikkComponent } from './historikk/historikk.component';
import { DekrypterDialogComponent } from './browse/dekrypter-dialog.component';
import { FolderExplorerComponent } from './browse/browse.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CdkTreeModule } from '@angular/cdk/tree';
import { MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon'
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HistorikkComponent,
    FolderExplorerComponent,
    DekrypterDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'bla', pathMatch: 'full' },
      { path: 'bla', component: FolderExplorerComponent },
      { path: 'historikk', component: HistorikkComponent }
    ]),
    BrowserAnimationsModule,
    CdkTreeModule,
    MatTreeModule,
    MatIconModule,
    CdkTreeModule,
    MatDialogModule,
    MatButtonModule,
    MatTreeModule,
    MatTooltipModule
  ],
  entryComponents: [DekrypterDialogComponent],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
