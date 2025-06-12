import { Injectable } from '@angular/core';
import { User } from '../models/user';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { HttpClient} from '@angular/common/http';
import { environment_development } from '../../environments/environment.development';
import { AuthRequest } from '../models/auth-request';
import { AuthResponse } from '../models/auth-response';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private BASE_URL = environment_development.apiUrl;
  // BehaviorSubject para gestionar el estado de autenticación de forma reactiva.
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  private isAdminSubject = new BehaviorSubject<boolean>(this.checkAdmin());

  constructor(private http: HttpClient) { }

  // Realiza la petición de login.
  login(authData: AuthRequest, rememberMe: boolean): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.BASE_URL}/api/User/login`, authData).pipe(
      tap((response: AuthResponse) => {
        // Limpia cualquier token anterior.
        localStorage.removeItem('accessToken');
        sessionStorage.removeItem('accessToken');
        localStorage.removeItem('isAdmin');
        sessionStorage.removeItem('isAdmin');
  
        // Almacena el nuevo token en localStorage o sessionStorage según la opción "Recuérdame".
        if (rememberMe) {
          localStorage.setItem('accessToken', response.stringToken);
          localStorage.setItem('isAdmin', JSON.stringify(response.isAdmin));
        } else {
          sessionStorage.setItem('accessToken', response.stringToken);
          sessionStorage.setItem('isAdmin', JSON.stringify(response.isAdmin));
        }
        
        // Notifica a los suscriptores que el estado de autenticación ha cambiado.
        this.loggedIn.next(true);
        this.isAdminSubject.next(this.checkAdmin()); 
      })
    );
  }

  // Realiza la petición de registro.
  register(formData: FormData): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.BASE_URL}/api/User/register`, formData, { headers: {} });
  }

  // Cierra la sesión del usuario.
  logout(): void {
    localStorage.removeItem('accessToken');
    sessionStorage.removeItem('accessToken');
    this.loggedIn.next(false);
    this.isAdminSubject.next(false);
    console.log('AuthService: Sesión cerrada correctamente');
  }

  // Comprueba si existe un token en el almacenamiento local.
  private hasToken(): boolean {
    return !!localStorage.getItem('accessToken');
  }

  // Comprueba si el usuario tiene el rol de administrador a partir del token.
  private checkAdmin(): boolean {
    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');
    if (!token) return false;
    
    try {
      const payload = this.decodeToken(token);
      return payload.role === 'admin';
    } catch (e) {
      console.error('Error decodificando el token:', e);
      return false;
    }
  }

  // Decodifica el payload de un token JWT.
  private decodeToken(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(atob(base64));
  }

  // Expone el estado de autenticación como un Observable.
  get isLoggedIn$(): Observable<boolean> {
    return this.loggedIn.asObservable();
  }

  // Expone el estado de administrador como un Observable.
  get isAdmin$(): Observable<boolean> {
    return this.isAdminSubject.asObservable();
  }

  // Actualiza el estado de autenticación (útil al iniciar la app).
  updateAuthState(): void {
    this.loggedIn.next(this.hasToken());
    this.isAdminSubject.next(this.checkAdmin());
  }

  // Obtiene los datos del usuario decodificando el token.
  getUserData(): { 
    id: number, 
    nickname: string, 
    email: string, 
    profilephoto: string,
    role: boolean 
  } | null {

    const token = localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken');

    if (!token) return null;

    try {
      const payload = this.decodeToken(token);
      return {
        id: payload.id,
        nickname: payload.nickname,
        email: payload.email,
        profilephoto: payload.profilephoto,
        role: payload.role === 'admin'
      };
    } catch (e) {
      console.error('Error obteniendo datos del usuario:', e);
      return null;
    }
  }
}