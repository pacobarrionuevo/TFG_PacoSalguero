import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { User } from '../../models/user';
import { SolicitudAmistad } from '../../models/solicitud-amistad';
import { FriendService } from '../../services/friend.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';

@Component({
  selector: 'app-amigos',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './amigos.component.html',
  styleUrls: ['./amigos.component.css']
})
export class AmigosComponent implements OnInit, OnDestroy {
  amigosFiltrados: User[] = [];
  solicitudesPendientes: SolicitudAmistad[] = [];
  usuariosFiltrados: User[] = [];
  todosLosUsuarios: User[] = [];
  
  terminoBusqueda: string = '';
  usuarioId: number | null = null;
  solicitudesEnviadas: number[] = [];
  perfil_default = 'assets/img/perfil_default.png';

  private subscriptions: Subscription = new Subscription();

  constructor(
    private friendService: FriendService,
    private userService: UserService,
    private authService: AuthService,
    private websocketService: WebsocketService
  ) {}

  ngOnInit(): void {
    this.usuarioId = this.authService.getUserData()?.id ?? null;
    this.cargarAmigos();
    this.cargarSolicitudes();
    this.cargarTodosLosUsuarios();
    this.suscribirAEventosWebSocket();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private cargarAmigos(): void {
    this.friendService.getFriendsList().subscribe(amigos => {
      this.amigosFiltrados = amigos;
    });
  }

  private cargarSolicitudes(): void {
    this.friendService.getPendingRequests().subscribe(solicitudes => {
      this.solicitudesPendientes = solicitudes;
    });
  }

  private cargarTodosLosUsuarios(): void {
    this.userService.getUsuarios().subscribe(usuarios => {
      this.todosLosUsuarios = usuarios;
    });
  }

  private suscribirAEventosWebSocket(): void {
    const friendStatusSub = this.websocketService.friendStatusUpdate$.subscribe(update => {
      const amigo = this.amigosFiltrados.find(a => a.UserId === update.userId);
      if (amigo) {
        amigo.isOnline = update.isOnline;
        amigo.lastSeen = update.lastSeen;
      }
    });

    const newFriendSub = this.websocketService.newFriendNotification$.subscribe(nuevoAmigo => {
        if (!this.amigosFiltrados.some(a => a.UserId === nuevoAmigo.UserId)) {
            this.amigosFiltrados.push(nuevoAmigo);
        }
    });

    this.subscriptions.add(friendStatusSub);
    this.subscriptions.add(newFriendSub);
  }

  aceptarSolicitud(solicitud: SolicitudAmistad): void {
    this.friendService.aceptarSolicitud(solicitud.friendshipId).subscribe(() => {
      this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.friendshipId !== solicitud.friendshipId);
    });
  }

  rechazarSolicitud(solicitud: SolicitudAmistad): void {
    this.friendService.rechazarSolicitud(solicitud.friendshipId).subscribe(() => {
      this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.friendshipId !== solicitud.friendshipId);
    });
  }

  enviarSolicitud(receiverId: number | undefined): void {
    if (receiverId === undefined) return;
    this.friendService.sendFriendRequest(receiverId).subscribe(() => {
      this.solicitudesEnviadas.push(receiverId);
    });
  }

  buscarUsuarios(): void {
    if (!this.terminoBusqueda) {
      this.usuariosFiltrados = [];
      return;
    }
    this.usuariosFiltrados = this.todosLosUsuarios.filter(u =>
      u.UserNickname?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  esAmigo(id: number | undefined): boolean {
    if (id === undefined) return false;
    return this.amigosFiltrados.some(a => a.UserId === id);
  }

  solicitudEnviada(id: number | undefined): boolean {
    if (id === undefined) return false;
    return this.solicitudesEnviadas.includes(id);
  }

  getStatusText(amigo: User): string {
    if (amigo.isOnline) {
      return 'Conectado';
    }
    if (!amigo.lastSeen) {
      return 'Desconectado';
    }

    const lastSeenDate = new Date(amigo.lastSeen);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - lastSeenDate.getTime()) / 1000);

    let interval = seconds / 31536000;
    if (interval > 1) return `Hace ${Math.floor(interval)} años`;
    
    interval = seconds / 2592000;
    if (interval > 1) return `Hace ${Math.floor(interval)} meses`;

    interval = seconds / 86400;
    if (interval > 1) return `Hace ${Math.floor(interval)} días`;

    interval = seconds / 3600;
    if (interval > 1) return `Hace ${Math.floor(interval)} horas`;

    interval = seconds / 60;
    if (interval > 1) return `Hace ${Math.floor(interval)} minutos`;

    return 'Hace un momento';
  }
}