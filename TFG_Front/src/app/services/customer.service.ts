import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { Customer } from '../models/customer';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  private baseURL = `${environment_development.apiUrl}/api/Customer`

  constructor(private http: HttpClient) { }

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.baseURL}`);
  }

  create(customer: Customer): Observable<Customer> {
    return this.http.post<Customer>(`${this.baseURL}`, customer);
  }

  edit(customer: Customer): Observable<Customer> {
    return this.http.put<Customer>(`${this.baseURL}`, customer);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseURL}/${id}`);
  }

}
