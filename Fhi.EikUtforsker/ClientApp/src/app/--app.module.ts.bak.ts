import { BrowserModule } from '@angular/platform-browser';
import { NgModule, APP_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
  
@NgModule({
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'bla', pathMatch: 'full' },
      { path: 'bla', loadComponent: () => import('./browse/browse.component').then(m => m.FolderExplorerComponent) },
      { path: 'historikk', loadComponent: () => import('./historikk/historikk.component').then(m => m.HistorikkComponent) },
    ]),
    BrowserAnimationsModule
  ],
  providers: [
    { provide: APP_ID, useValue: 'eik-utforsker' }
  ]
})
export class AppModule { }
