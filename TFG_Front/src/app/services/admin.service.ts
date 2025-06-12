import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment_development.apiUrl}/api/admin`;

  constructor(private http: HttpClient) {}

  // Método privado para obtener las cabeceras de autenticación.
  // Es crucial para que el backend reconozca al usuario.
  private getAuthHeaders(isFormData: boolean = false): HttpHeaders {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    // No se establece 'Content-Type' si es FormData, el navegador lo hace automáticamente.
    if (!isFormData) {
      headers = headers.set('Content-Type', 'application/json');
    }
    return headers;
  }

  // Obtiene las estadísticas para el dashboard del administrador.
  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard-stats`, { headers: this.getAuthHeaders() });
  }

  // Cambia el rol de un usuario.
  changeUserRole(userId: number, role: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/role`, { role }, { headers: this.getAuthHeaders() });
  }

  // Actualiza los datos de un usuario (nickname, email).
  updateUser(userId: number, userData: { userNickname: string; userEmail: string; }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/details`, userData, { headers: this.getAuthHeaders() });
  }

  // Actualiza el avatar de un usuario.
  updateUserAvatar(userId: number, avatar: File): Observable<{ filePath: string }> {
    const formData = new FormData();
    formData.append('avatar', avatar, avatar.name);
    // Se pasa 'true' para indicar que es una petición FormData.
    return this.http.put<{ filePath: string }>(`${this.apiUrl}/users/${userId}/avatar`, formData, { headers: this.getAuthHeaders(true) });
  }

  // Elimina un usuario.
  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${userId}`, { headers: this.getAuthHeaders() });
  }
}