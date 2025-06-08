import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Service } from '../models/service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServicesService {

  private baseURL = 'https://localhost:7077/api/Service';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Service[]> {
    return this.http.get<Service[]>(`${this.baseURL}/get_services`);
  }

  create(service: Service): Observable<Service> {
    return this.http.post<Service>(`${this.baseURL}/post_services`, service);
  }
  
  update(service: Service): Observable<Service> {
    return this.http.put<Service>(`${this.baseURL}/${service.id}`, service);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseURL}/${id}`);
  }
}
