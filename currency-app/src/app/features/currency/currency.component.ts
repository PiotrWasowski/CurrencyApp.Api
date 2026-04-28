import { Component, OnInit } from '@angular/core';
import { CurrencyService } from './currency.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-currency',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './currency.component.html'
})
export class CurrencyComponent implements OnInit {
  form: any;
  constructor(
    private service: CurrencyService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      apiType: ['Nbp'],
      from: [''],
      to: [''],
      fromDate: [''],
      toDate: ['']
    });
  }

  currencies: any[] = [];
  result: any;
  loading = false;
  error = '';

  

  ngOnInit() {
    this.loadCurrencies();
  }

  loadCurrencies() {
    const api = this.form.value.apiType!;
    this.service.getCurrencies(api).subscribe({
      next: res => this.currencies = res,
      error: () => this.error = 'Błąd pobierania walut'
    });
  }

  submit() {
    this.loading = true;
    this.error = '';

    this.service.getRates(this.form.value).subscribe({
      next: res => {
        this.result = res;
        this.loading = false;
      },
      error: () => {
        this.error = 'Błąd pobierania danych';
        this.loading = false;
      }
    });
  }
}
