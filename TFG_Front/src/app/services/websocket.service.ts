import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { User } from '../models/user';
import { SolicitudAmistad } from '../models/solicitud-amistad';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private socket: WebSocket | null = null;
  private friendStatusUpdateSubject = new Subject<any>();
  private newFriendNotificationSubject = new Subject<User>();

  public friendStatusUpdate$ = this.friendStatusUpdateSubject.asObservable();
  public newFriendNotification$ = this.newFriendNotificationSubject.asObservable();
  private newFriendRequestSubject = new Subject<SolicitudAmistad>();  constructor() { }
public newFriendRequest$ = this.newFriendRequestSubject.asObservable();
  public connectRxjs(token: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      console.log('[WS Service] Conexión ya existente.');
      return;
    }

    const url = `${environment_development.socketUrl}?token=${token}`;
    console.log(`[WS Service] Intentando conectar a: ${url}`);
    this.socket = new WebSocket(url);

    this.socket.onopen = () => {
      console.log('[WS Service] Conexión WebSocket establecida con éxito.');
    };

    this.socket.onmessage = (event) => {
      console.log(`[WS Service] Mensaje recibido del servidor:`, event.data);
      try {
        const message = JSON.parse(event.data);
        this.handleMessage(message);
      } catch (error) {
        console.error('[WS Service] Error parseando mensaje JSON:', error);
      }
    };

    this.socket.onclose = (event) => {
      console.log('[WS Service] Conexión WebSocket cerrada.', event);
      this.socket = null;
    };

    this.socket.onerror = (error) => {
      console.error('[WS Service] Error de WebSocket:', error);
    };
  }

  public send(message: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      console.log(`[WS Service] Enviando mensaje al servidor:`, message);
      this.socket.send(message);
    } else {
      console.error('[WS Service] WebSocket no está conectado. Mensaje no enviado:', message);
    }
  }

  private handleMessage(message: any): void {
    if (!message.type || !message.payload) {
      console.warn('[WS Service] Mensaje recibido con formato incorrecto:', message);
      return;
    }

    console.log(`[WS Service] Manejando mensaje tipo: ${message.type}`);
    switch (message.type) {
      case 'friend_status_update':
        this.friendStatusUpdateSubject.next(message.payload);
        break;
      case 'new_friend_notification':
        this.newFriendNotificationSubject.next(message.payload);
        break;
      case 'new_friend_request': 
        this.newFriendRequestSubject.next(message.payload);
        break;
      default:
        console.log(`[WS Service] Tipo de mensaje no manejado: ${message.type}`);
        break;
    }
  }

  public disconnect(): void {
    if (this.socket) {
      console.log('[WS Service] Desconectando WebSocket.');
      this.socket.close();
    }
  }
}