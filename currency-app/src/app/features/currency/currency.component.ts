import * as TestReporters from 'node:test/reporters';
import { Component, OnInit } from '@angular/core';
import { CurrencyService } from './currency.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Observable, of } from 'rxjs';
import { tap, catchError, finalize } from 'rxjs/operators';

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
    
  result: any;
  loading = false;
  error = '';

  
  ngOnInit() {
    this.loadCurrencies();
  }

  currencies$!: Observable<any[]>;

  loadCurrencies() {
    const api = this.form.value.apiType!;
    this.currencies$ = this.service.getCurrencies(api).pipe(
      catchError(err => {
        this.error = 'Nie udało się pobrać listy walut';
        return of([]);
      })
    );;
  }

  result$!: Observable<any>;

  submit() {
    this.loading = true;
    this.error = '';
    this.result = null;

    this.result$ = this.service.getRates(this.form.value).pipe(
      tap(res => {
        if (!res) {
          this.error = 'Brak danych dla wybranego zakresu';
        }
        else { 
          this.error = '';
        }
      }),
      catchError(err => {
        this.handleError(err);
        return of(null); 
      }),
      finalize(() => {
        this.loading = false;
      })
    );
  }

  handleError(error: any) {
    console.log(error.status);
    if (error.status === 0) {
      this.error = 'Brak połączenia z serwerem';
    } else if (error.status === 400) {
      this.error = error.error || 'Nieprawidłowe dane';
    }  else {
      this.error = 'Błąd serwera';
    }
  }
}
