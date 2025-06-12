import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Result } from '../models/result';
import { Observable, lastValueFrom } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
// Servicio genérico para interactuar con la API, estandarizando las peticiones y respuestas.
export class ApiService {

  private BASE_URL = environment_development.apiUrl;

  jwt: string | undefined;

  constructor(private http: HttpClient) { }

  // --- Métodos específicos (podrían refactorizarse para usar los métodos genéricos) ---
   getUsuarios(): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE_URL}/api/User/users`);
  }

  getFriendsList(usuarioId: number): Observable<User[]> {
    return this.http.get<User[]>(`${this.BASE_URL}/api/FriendRequest/friends/${usuarioId}`);
  }

  getPendingFriendRequests(usuarioId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.BASE_URL}/api/FriendRequest/pending/${usuarioId}`);
  }

  sendFriendRequest(receiverId: number): Observable<any> {
    const params = new HttpParams().set('receiverId', receiverId.toString());
    return this.http.post(`${this.BASE_URL}/api/FriendRequest/send`, {}, {
      headers: this.getHeader(),
      params: params
    });
  }
  
  acceptFriendRequest(amistadId: number): Observable<any> {
    return this.http.post(`${this.BASE_URL}/api/FriendRequest/accept`, { amistadId }, {
      headers: this.getHeader()
    });
  }
  
  rejectFriendRequest(amistadId: number): Observable<any> {
    return this.http.post(`${this.BASE_URL}/api/FriendRequest/reject`, null, {
      headers: this.getHeader(),
      params: { amistadId: amistadId.toString() }
    });
  }

  getUsuarioById(userId: number): Observable<User> {
    return this.http.get<User>(`${this.BASE_URL}/api/User/users/${userId}`);
  }

  actualizarUsuario(userId: number, datos: any): Observable<any> {
    return this.http.put(`${this.BASE_URL}/api/User/users/${userId}`, datos);
  }
  
  // --- Métodos genéricos para peticiones HTTP ---

  async get<T = void>(path: string, params: any = {}, responseType: 'json' | 'text' | 'blob' | 'arraybuffer' = 'json'): Promise<Result<T>> {
    const url = `${this.BASE_URL}/api${path}`;
    const request$ = this.http.get(url, {
      params: new HttpParams({ fromObject: params }),
      headers: this.getHeader(),
      responseType: responseType as any,
      observe: 'response',
    });
    return this.sendRequest<T>(request$);
  }

  async post<T = void>(path: string, body: Object = {}, contentType = null): Promise<Result<T>> {
    const url = `${this.BASE_URL}${path}`;
    const request$ = this.http.post(url, body, {
      headers: this.getHeader(contentType),
      observe: 'response'
    });
    return this.sendRequest<T>(request$);
  }

  async put<T = void>(path: string, body: Object = {}, contentType = null): Promise<Result<T>> {
    const url = `${this.BASE_URL}${path}`;
    const request$ = this.http.put(url, body, {
      headers: this.getHeader(contentType),
      observe: 'response'
    });
    return this.sendRequest<T>(request$);
  }

  async delete<T = void>(path: string, params: any = {}): Promise<Result<T>> {
    const url = `${this.BASE_URL}${path}`;
    const request$ = this.http.delete(url, {
      params: new HttpParams({ fromObject: params }),
      headers: this.getHeader(),
      observe: 'response'
    });
    return this.sendRequest<T>(request$);
  }

  // Envuelve la petición HTTP en una promesa y la convierte a un objeto Result.
  private async sendRequest<T = void>(request$: Observable<HttpResponse<any>>): Promise<Result<T>> {
    let result: Result<T>;
    
    try {
      const response = await lastValueFrom(request$);
      const statusCode = response.status;

      if (response.ok) {
        const data = response.body as T;
        result = data == undefined ? Result.success(statusCode) : Result.success(statusCode, data);
      } else {
        result = Result.error(statusCode, response.statusText);
      }
    } catch (exception) {
      if (exception instanceof HttpErrorResponse) {
        result = Result.error(exception.status, exception.statusText);
      } else if (exception instanceof Error) {
        result = Result.error(-1, exception.message || 'Unknown error');
      } else {
        result = Result.error(-1);
      }
    }

    return result;
  }

  // Construye las cabeceras HTTP, añadiendo el token de autorización si existe.
  private getHeader(contentType: string | null = 'application/json'): HttpHeaders {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    let header: any = {};
  
    if (token) {
      header['Authorization'] = `Bearer ${token}`;  
    }
    if (contentType) {
      header['Content-Type'] = contentType;
    }
    return new HttpHeaders(header);
  }
}