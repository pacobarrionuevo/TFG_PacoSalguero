import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { environment_development } from '../../environments/environment.development';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class WebsocketService {
  private socket: WebSocket | null = null;
  private friendStatusUpdateSubject = new Subject<any>();
  private newFriendNotificationSubject = new Subject<User>();

  public friendStatusUpdate$ = this.friendStatusUpdateSubject.asObservable();
  public newFriendNotification$ = this.newFriendNotificationSubject.asObservable();

  constructor() { }

  public connectRxjs(token: string): void {
    if (this.socket && this.socket.readyState === WebSocket.OPEN) {
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
        this.handleMessage(message);
      } catch (error) {
        console.error('Error parsing WebSocket message:', error);
      }
    };

    this.socket.onclose = (event) => {
      console.log('WebSocket connection closed:', event);
      this.socket = null;
    };

    this.socket.onerror = (error) => {
      console.error('WebSocket error:', error);
    };
  }

  private handleMessage(message: any): void {
    if (!message.type || !message.payload) {
      console.warn('Received malformed WebSocket message:', message);
      return;
    }

    switch (message.type) {
      case 'friend_status_update':
        this.friendStatusUpdateSubject.next(message.payload);
        break;
      case 'new_friend_notification':
        this.newFriendNotificationSubject.next(message.payload);
        break;
      default:
        console.log('Received unhandled message type:', message.type);
        break;
    }
  }

  public disconnect(): void {
    if (this.socket) {
      this.socket.close();
    }
  }
}