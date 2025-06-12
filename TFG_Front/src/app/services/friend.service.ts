import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { SolicitudAmistad } from '../models/solicitud-amistad';
import { SendFriendRequest } from '../models/send-friend-request';
import { User } from '../models/user';
import { environment_development } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las interacciones de amistad (solicitudes, lista de amigos, etc.).
export class FriendService {
  private apiUrl = `${environment_development.apiUrl}/api/FriendRequest`;
  
  constructor(private http: HttpClient) { }

  // Obtiene la lista de amigos del usuario autenticado.
  getFriendsList(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/friends`, {
      headers: this.getHeader()
    });
  }

  // Obtiene las solicitudes de amistad pendientes para el usuario autenticado.
  getPendingRequests(): Observable<SolicitudAmistad[]> {
    return this.http.get<SolicitudAmistad[]>(`${this.apiUrl}/pending`, {
      headers: this.getHeader()
    });
  }

  // Envía una solicitud de amistad a un usuario.
  sendFriendRequest(receiverId: number): Observable<SendFriendRequest> {
    const params = new HttpParams().set('receiverId', receiverId.toString());
    return this.http.post<SendFriendRequest>(`${this.apiUrl}/send`, {}, {
      headers: this.getHeader(),
      params: params
    });
  }
  
  // Acepta una solicitud de amistad.
  aceptarSolicitud(friendshipId: number | null): Observable<any> {
    if (friendshipId == null) {
      console.error('Error: amistadId es null o undefined');
      return throwError(() => new Error('amistadId es null o undefined'));
    }
    
    const params = new HttpParams().set('amistadId', friendshipId.toString());
    return this.http.post(`${this.apiUrl}/accept`, null, {
      headers: this.getHeader(),
      params: params
    });
  }
  
  // Rechaza una solicitud de amistad.
  rechazarSolicitud(friendshipId: number): Observable<any> {
    const params = new HttpParams().set('amistadId', friendshipId.toString());
    return this.http.post(`${this.apiUrl}/reject`, null, {
      headers: this.getHeader(),
      params: params
    });
  }

  // Método privado para construir las cabeceras de autenticación.
  private getHeader(contentType: string | null = 'application/json'): HttpHeaders {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    let headers: any = {};
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
    if (contentType) {
      headers['Content-Type'] = contentType;
    }
    return new HttpHeaders(headers);
  }
}