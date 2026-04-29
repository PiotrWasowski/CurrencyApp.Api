import { Component, OnInit } from '@angular/core';
import { CurrencyService } from './currency.service';
import { FormBuilder, ReactiveFormsModule, FormGroup, AbstractControl, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Observable, of } from 'rxjs';
import { tap, catchError, finalize } from 'rxjs/operators';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatOptionModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-currency',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatOptionModule,
    MatSelectModule,
    MatInputModule,
    MatCardModule
  ],
  templateUrl: './currency.component.html'
})
export class CurrencyComponent implements OnInit {

  form: FormGroup;

  result: any;
  loading = false;
  error = '';

  currencies$!: Observable<any[]>;
  result$!: Observable<any>;
  today = new Date();

  constructor(
    private service: CurrencyService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      apiType: ['Nbp', Validators.required],
      from: ['', Validators.required],
      to: ['', Validators.required],
      fromDate: [this.today, [Validators.required, this.futureDateValidator.bind(this)]],
      toDate: [this.today, [Validators.required, this.futureDateValidator.bind(this)]]
    }, { validators: this.dateRangeValidator });
  }

  ngOnInit() {
    this.loadCurrencies();
  }

  dateRangeValidator(group: FormGroup) {
    const from = group.get('fromDate')?.value.setHours(0, 0, 0, 0);
    const to = group.get('toDate')?.value.setHours(0, 0, 0, 0);

    if (from && to && new Date(from) > new Date(to)) {
      return { dateRange: true };
    }

    return null;
  }

  futureDateValidator(control: AbstractControl) {
    if (control.value && new Date(control.value) > new Date()) {
      return { futureDate: true };
    }
    return null;
  }

  loadCurrencies() {
    const api = this.form.value.apiType!;

    this.currencies$ = this.service.getCurrencies(api).pipe(
      catchError(() => {
        this.error = 'Nie udało się pobrać listy walut';
        return of([]);
      })
    );
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  }

  submit() {
    this.loading = true;
    this.error = '';
    this.result = null;

    const formValue = this.form.value;

    const today = new Date();
    const fromDate = formValue.fromDate || today;
    const toDate = formValue.toDate || today;

    const request = {
      ...formValue,
      fromDate: this.formatDate(fromDate),
      toDate: this.formatDate(toDate)
    };

    this.result$ = this.service.getRates(request).pipe(
      tap(res => {
        this.error = res ? '' : 'Brak danych dla wybranego zakresu';
      }),
      catchError(err => {
        this.handleError(err);
        return of(null);
      }),
      finalize(() => {
        this.loading = false;
        if (!formValue.fromDate || !formValue.toDate) {
          this.form.patchValue({
            fromDate: fromDate,
            toDate: toDate
          });
        }
      })
    );
  }

  handleError(error: any) {
    console.log(error.status);

    if (error.status === 0) {
      this.error = 'Brak połączenia z serwerem';
    } else if (error.status === 400) {
      this.error = error.error || 'Nieprawidłowe dane';
    } else {
      this.error = 'Błąd serwera';
    }
  }
}
