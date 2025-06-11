import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { Subject, BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { environment_development } from '../../environments/environment.development';
export interface UserStatusUpdate {
  userId: number;
  isOnline: boolean;
}
@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private socket?: WebSocket;
  private userStatusChanged = new Subject<UserStatusUpdate>();
  
  public userStatusChanged$ = this.userStatusChanged.asObservable();
  // Variables para gestionar la conexión, si se ha recibido el mensaje, si se ha desconectado o si hay alguna conexión activa
  connected = new Subject<void>(); 
  messageReceived = new Subject<any>();
  disconnected = new Subject<void>();
  activeConnections = new Subject<number>();

  friendNotAvailable = new Subject<string>();
  public onlineUsers$ = new BehaviorSubject<Set<number>>(new Set());

  // Clave para almacenar el token en sessionStorage
  private tokenKey = 'websocket_token'; 

  // Conexion ws
  private rxjsSocket: WebSocketSubject<string>;

  private baseURL = environment_development.apiUrl;
  private socketURL = environment_development.socketUrl;

  

 
  
  constructor(private http: HttpClient, private router: Router, private ngZone: NgZone) {
    // Imaginando que fallase la conexión, este método intenta reconectar al iniciar el servicio
    this.reconnectIfNeeded(); 
  }

  // Método para asegurar la conexión del websocket
  private onConnected() {
    console.log('WebSocketService: Socket connected');
    this.connected.next();
  }

  // Verifica si el websocket está conectado.
  isConnectedRxjs(): boolean {
    const isConnected = this.rxjsSocket && !this.rxjsSocket.closed;
    console.log(`WebSocketService: isConnectedRxjs() -> ${isConnected}`);
    return isConnected;
  }

  // Conecta el websocket usando el token de autenticación del usuario que inició sesión.
  public connectRxjs(token: string): void {
    if (this.socket && (this.socket.readyState === WebSocket.OPEN || this.socket.readyState === WebSocket.CONNECTING)) {
      console.log('WebSocket connection already open or connecting.');
      return;
    }

    const url = `${environment_development.socketUrl}?token=${token}`;
    this.socket = new WebSocket(url);

    this.socket.onopen = () => {
      console.log('WebSocket connection established');
    };

    this.socket.onmessage = (event) => {
      try {
        const message = JSON.parse(event.data);
        if (message.type === 'status_update') {
          this.userStatusChanged.next({ userId: message.userId, isOnline: message.isOnline });
        }
      } catch (error) {
        console.error('Error parsing WebSocket message:', error);
      }
    };

    this.socket.onclose = (event) => {
      console.log('WebSocket connection closed:', event.reason);
      this.socket = undefined;
    };

    this.socket.onerror = (error) => {
      console.error('WebSocket error:', error);
    };
  }

  // Envía un mensaje a través del websocket
  sendRxjs(message: string) {
    if (this.rxjsSocket && !this.rxjsSocket.closed) {
      console.log('WebSocketService: Enviando mensaje:', message);
      this.rxjsSocket.next(message);
    } else {
      console.error('WebSocketService: No se puede enviar el mensaje, WebSocket no está conectado');
    }
  }

  // Desconecta el websocket
  disconnectRxjs() {
    if (this.rxjsSocket) {
      console.log('WebSocketService: Desconectando WebSocket...');
      this.onDisconnected();
      this.rxjsSocket.complete();
      this.rxjsSocket.unsubscribe();
    }
  }
  

  // Muestra que se ha desconectado el websocket
  private onDisconnected() {
    console.log('WebSocketService: Desconectado del WebSocket');
    this.disconnected.next();
  }
  public disconnect(): void {
    if (this.socket) {
      this.socket.close();
      this.socket = undefined;
    }
  
}

// Maneja todos los posibles mensajes recibidos del websockets
private handleMessage(message: string): void {
  try {
      // Intentar parsear el mensaje como JSON
      const parsedMessage = JSON.parse(message);
      console.log('WebSocketService: Mensaje recibido:', parsedMessage);

      // Convertir propiedades a camelCase si es necesario
      const normalizedMessage = this.normalizeKeys(parsedMessage);

      // Procesar el mensaje según su tipo
        if (normalizedMessage.type === 'activeConnections') {
          this.handleActiveConnections(normalizedMessage);
      }  else if (normalizedMessage.type === 'friendConnected') {
          this.addOnlineUser(normalizedMessage.friendId);
      } else if (normalizedMessage.type === 'friendDisconnected') {
          this.removeOnlineUser(normalizedMessage.friendId);
      } else if (normalizedMessage.type === 'onlineUsers') {
          this.handleOnlineUsers(normalizedMessage);
      } else if (normalizedMessage.type === 'friendRequest') {
          // Manejar solicitud de amistad
          this.messageReceived.next({
            type: 'friendRequest',
            requestId: parsedMessage.requestId,
            senderId: parsedMessage.senderId,
            senderName: parsedMessage.senderName
          });
      } else if (normalizedMessage.type === 'friendRequestAccepted') {
        this.messageReceived.next({
          type: 'friendRequestAccepted',
          friendId: parsedMessage.friendId,
          friendName: parsedMessage.friendName
        });
          this.messageReceived.next(normalizedMessage);
      } else if (normalizedMessage.type === 'friendRequestRejected') {
        this.messageReceived.next({
          type: 'friendRequestRejected',
          friendId: parsedMessage.friendId
        });
          this.messageReceived.next(normalizedMessage);
      } else if (normalizedMessage.type === 'friendListUpdate') {
          
          console.log('Lista de amigos actualizada:', normalizedMessage);
          this.messageReceived.next(normalizedMessage);
      }  else {
          console.log('WebSocketService: Mensaje recibido no manejado:', normalizedMessage);
      }
    } catch (error) {
        console.error('Error al parsear el mensaje:', error);
    }
  }

  // Maneja el número de conexiones activas
  private handleActiveConnections(parsedMessage: any): void {
    console.log(`WebSocketService: Recibido activeConnections. Count: ${parsedMessage.count}`);
    this.activeConnections.next(parsedMessage.count);
  }
fetchOnlineUsers(): Observable<number[]> {
    return this.http.get<{ onlineUsers: number[] }>(`${this.baseURL}/ws/online-users`).pipe(
      map(response => Array.isArray(response?.onlineUsers) ? response.onlineUsers : []),
      catchError(() => of([]))
    );
  }

  // Maneja el estado de conexión de un amigo
  private handleFriendStatus(message: any) {
    console.log(`WebSocketService: Evento de amigo ${message.type} recibido para el ID: ${message.friendId}`);
    this.messageReceived.next(message);
  }

  // Maneja errores en la conexión websocket
  private onError(error: any) {
    console.error('WebSocketService: Error en la conexión WebSocket:', error);
    this.disconnected.next();
  }

  // Normaliza las claves de un objeto a camelCase
  private normalizeKeys(obj: any): any {
    if (typeof obj !== 'object' || obj === null) return obj;

    return Object.keys(obj).reduce((acc: any, key) => {
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
      acc[camelKey] = this.normalizeKeys(obj[key]);
      return acc;
    }, {});
  }

  private addOnlineUser(userId: number): void {
    this.ngZone.run(() => {
      const updated = new Set(this.onlineUsers$.value);
      updated.add(userId);
      this.onlineUsers$.next(updated);
    });
  }

  private removeOnlineUser(userId: number): void {
    this.ngZone.run(() => {
      const updated = new Set(this.onlineUsers$.value);
      updated.delete(userId);
      this.onlineUsers$.next(updated);
    });
  }

  private handleOnlineUsers(message: { users: number[] }): void {
    this.ngZone.run(() => {
      this.onlineUsers$.next(new Set(message.users));
    });
  }

  // Intenta reconectar el websocket si hay un token almacenado
  private reconnectIfNeeded() {
    const storedToken = sessionStorage.getItem(this.tokenKey);
    if (storedToken) {
      console.log('WebSocketService: Reconectando WebSocket con token almacenado');
      this.connectRxjs(storedToken); // Intentar reconectar
    } else {
      console.log('WebSocketService: No hay token almacenado, no se puede reconectar');
    }
  }

  // Elimina el token almacenado (por ejemplo, al cerrar sesión)
  clearToken() {
    sessionStorage.removeItem(this.tokenKey);
    console.log('WebSocketService: Token eliminado');
  }
}