import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { PaymentMethod } from '../models/payment-method';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las operaciones CRUD de los métodos de pago.
export class PaymentMethodService {

  private baseURL = `${environment_development.apiUrl}/api/PaymentMethod`

  constructor(private http: HttpClient) { }

  getAll(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(`${this.baseURL}`);
  }

  create(paymentMethod: PaymentMethod): Observable<PaymentMethod> {
    return this.http.post<PaymentMethod>(`${this.baseURL}`, paymentMethod);
  }

  update(paymentMethod: PaymentMethod): Observable<any> {
    return this.http.put(`${this.baseURL}/${paymentMethod.id}`, paymentMethod);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseURL}/${id}`);
  }

}