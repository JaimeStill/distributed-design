import { Component } from '@angular/core';
import { ThemeService } from '@distributed/toolkit';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [ThemeService]
})
export class AppComponent {
  constructor(
    public themer: ThemeService
  ) { }
}
