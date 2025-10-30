import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [
    CommonModule,
    RouterModule,
    NavMenuComponent
]
})
export class AppComponent {
  title = 'app';
}
