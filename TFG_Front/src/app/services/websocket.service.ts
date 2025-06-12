import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { User } from '../models/user';
import { SolicitudAmistad } from '../models/solicitud-amistad';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar la conexión y comunicación por WebSocket.
export class WebsocketService {
  private socket: WebSocket | null = null;
  // Subjects de RxJS para emitir eventos a los componentes que se suscriban.
  private friendStatusUpdateSubject = new Subject<any>();
  private newFriendNotificationSubject = new Subject<User>();
  private newFriendRequestSubject = new Subject<SolicitudAmistad>();

  // Observables públicos para que los componentes se suscriban.
  public friendStatusUpdate$ = this.friendStatusUpdateSubject.asObservable();
  public newFriendNotification$ = this.newFriendNotificationSubject.asObservable();
  public newFriendRequest$ = this.newFriendRequestSubject.asObservable();

  constructor() { }

  // Establece la conexión WebSocket.
  public connectRxjs(token: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      console.log('[WS Service] Conexión ya existente.');
      return;
    }

    // La URL incluye el token de autenticación en la query string.
    const url = `${environment_development.socketUrl}?token=${token}`;
    console.log(`[WS Service] Intentando conectar a: ${url}`);
    this.socket = new WebSocket(url);

    this.socket.onopen = () => {
      console.log('[WS Service] Conexión WebSocket establecida con éxito.');
    };

    // Maneja los mensajes entrantes del servidor.
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

  // Envía un mensaje al servidor a través del WebSocket.
  public send(message: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
      console.log(`[WS Service] Enviando mensaje al servidor:`, message);
      this.socket.send(message);
    } else {
      console.error('[WS Service] WebSocket no está conectado. Mensaje no enviado:', message);
    }
  }

  // Procesa los mensajes recibidos y emite eventos a través de los Subjects correspondientes.
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

  // Cierra la conexión WebSocket.
  public disconnect(): void {
    if (this.socket) {
      console.log('[WS Service] Desconectando WebSocket.');
      this.socket.close();
    }
  }
}