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

  private getAuthHeaders(isFormData: boolean = false): HttpHeaders {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    if (!isFormData) {
      headers = headers.set('Content-Type', 'application/json');
    }
    return headers;
  }

  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard-stats`, { headers: this.getAuthHeaders() });
  }

  changeUserRole(userId: number, role: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/role`, { role }, { headers: this.getAuthHeaders() });
  }

  // --- CORRECCIÓN AQUÍ ---
  // La ruta debe ser 'users/{id}/details' para coincidir con el backend
  updateUser(userId: number, userData: { userNickname: string; userEmail: string; }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/users/${userId}/details`, userData, { headers: this.getAuthHeaders() });
  }

  updateUserAvatar(userId: number, avatar: File): Observable<{ filePath: string }> {
    const formData = new FormData();
    formData.append('avatar', avatar, avatar.name);
    return this.http.put<{ filePath: string }>(`${this.apiUrl}/users/${userId}/avatar`, formData, { headers: this.getAuthHeaders(true) });
  }

  deleteUser(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/users/${userId}`, { headers: this.getAuthHeaders() });
  }
}