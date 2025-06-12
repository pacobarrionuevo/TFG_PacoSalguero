import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { Subject, BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { environment_development } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {

  // Variables para gestionar la conexión, si se ha recibido el mensaje, si se ha desconectado o si hay alguna conexión activa
  connected = new Subject<void>(); 
  messageReceived = new Subject<any>();
  disconnected = new Subject<void>();
  activeConnections = new Subject<number>();

  friendNotAvailable = new Subject<string>();
public onlineUsers$ = new BehaviorSubject<{ userId: number }[]>([]);
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
    this.fetchOnlineUsers().subscribe(users => {
      this.onlineUsers$.next(users);
    });
  }

  // Verifica si el websocket está conectado.
  isConnectedRxjs(): boolean {
    const isConnected = this.rxjsSocket && !this.rxjsSocket.closed;
    console.log(`WebSocketService: isConnectedRxjs() -> ${isConnected}`);
    return isConnected;
  }

  // Conecta el websocket usando el token de autenticación del usuario que inició sesión.
  connectRxjs(token: string) {
    console.log('WebSocketService: Conectando WebSocket con token:', token);

    // Verificar que el token sea una cadena válida
    if (typeof token !== 'string') {
      console.error('WebSocketService: El token no es una cadena válida:', token);
      return;
    }

    // Almacenar el token para reconexiones futuras --> recargas de páginas
    sessionStorage.setItem(this.tokenKey, token);

    // Cerrar la conexión existente si hay una
    if (this.rxjsSocket && !this.rxjsSocket.closed) {
      console.log('WebSocketService: Cerrando conexión WebSocket existente...');
      this.disconnectRxjs();
    }

    // Crear una nueva conexión websocket
    this.rxjsSocket = webSocket({
      url: `${this.socketURL}?token=${token}`,
      openObserver: { next: () => this.onConnected() },
      closeObserver: { next: (event: CloseEvent) => this.onDisconnected() },
      serializer: (value: string) => value,
      deserializer: (event: MessageEvent) => event.data
    });

    // Suscribirse a los mensajes del websocket
    this.rxjsSocket.subscribe({
      next: (message: string) => this.handleMessage(message),
      error: (error) => this.onError(error),
      complete: () => this.onDisconnected()
    });
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
    this.onlineUsers$.next([]);
    this.disconnected.next();
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
      }else if (message === 'user_status_changed') {
        console.log('Recibida notificación de cambio de estado. Actualizando lista de usuarios online...');
        this.fetchOnlineUsers().subscribe(users => {
            this.onlineUsers$.next(users);
        });
        return;
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
fetchOnlineUsers(): Observable<{ userId: number }[]> {
    return this.http.get<{ onlineUsers: { userId: number }[] }>(`${this.baseURL}/ws/online-users`).pipe(
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
      const currentUsers = this.onlineUsers$.value;
      if (!currentUsers.some(u => u.userId === userId)) {
        const updatedUsers = [...currentUsers, { userId }];
        this.onlineUsers$.next(updatedUsers);
      }
    });
  }

  private removeOnlineUser(userId: number): void {
    this.ngZone.run(() => {
      const updatedUsers = this.onlineUsers$.value.filter(u => u.userId !== userId);
      this.onlineUsers$.next(updatedUsers);
    });
  }


  private handleOnlineUsers(message: { users: { userId: number }[] }): void {
    this.ngZone.run(() => {
      this.onlineUsers$.next(message.users);
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