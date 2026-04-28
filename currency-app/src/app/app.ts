import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CurrencyComponent } from './features/currency/currency.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CurrencyComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('currency-app');
}
