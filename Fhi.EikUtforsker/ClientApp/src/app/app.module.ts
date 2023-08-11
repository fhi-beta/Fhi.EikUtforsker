import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { DekrypterDialogComponent } from './browse/dekrypter-dialog.component';
import { FolderExplorerComponent } from './browse/browse.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FolderEntryComponent } from './browse/folder-entry.component';
import { HistorikkEntryComponent } from './historikk/historikk-entry.component';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatExpansionModule } from '@angular/material/expansion';
import { HistorikkComponent } from './historikk/historikk.component';
  
@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    FolderExplorerComponent,
    HistorikkComponent,
    HistorikkEntryComponent,
    FolderEntryComponent,
    DekrypterDialogComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'bla', pathMatch: 'full' },
      { path: 'bla', component: FolderExplorerComponent },
      { path: 'historikk', component: HistorikkComponent },
    ]),
    MatListModule,
    MatDialogModule,
    MatIconModule,
    MatDialogModule,
    MatExpansionModule,
    MatTooltipModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
