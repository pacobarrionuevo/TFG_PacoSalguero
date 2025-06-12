import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
// Servicio para obtener información de usuarios.
export class UserService {
  private baseURL = `${environment_development.apiUrl}/api/User`; 

  constructor(private http: HttpClient) { }

  // Obtiene la lista completa de usuarios.
  getUsuarios(): Observable<User[]> {
    return this.http.get<User[]>(`${this.baseURL}/users`);
  }
}