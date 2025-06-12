import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { Customer } from '../models/customer';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las operaciones CRUD de los clientes.
export class CustomerService {

  private baseURL = `${environment_development.apiUrl}/api/Customer`

  constructor(private http: HttpClient) { }

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.baseURL}`);
  }

  create(customer: Customer): Observable<Customer> {
    return this.http.post<Customer>(`${this.baseURL}`, customer);
  }

  update(customer: Customer): Observable<any> {
    return this.http.put(`${this.baseURL}/${customer.id}`, customer);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseURL}/${id}`);
  }

}