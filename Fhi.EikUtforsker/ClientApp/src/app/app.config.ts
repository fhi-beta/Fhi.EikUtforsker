import { provideHttpClient } from '@angular/common/http';
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter([
      { path: '', redirectTo: 'bla', pathMatch: 'full' },
      { path: 'bla', loadComponent: () => import('./browse/browse.component').then(m => m.FolderExplorerComponent) },
      { path: 'historikk', loadComponent: () => import('./historikk/historikk.component').then(m => m.HistorikkComponent) },
    ]),
    provideHttpClient(),
    { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] }
  ]
};
