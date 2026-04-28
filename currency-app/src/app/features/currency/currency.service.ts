import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {
  private baseUrl = 'https://localhost:7089/api/currency'; 

  constructor(private http: HttpClient) { }

  getCurrencies(apiType: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/currencies`, {
      params: { apiType }
    });
  }

  getRates(params: any): Observable<any> {
    return this.http.get(this.baseUrl, { params });
  }
}
