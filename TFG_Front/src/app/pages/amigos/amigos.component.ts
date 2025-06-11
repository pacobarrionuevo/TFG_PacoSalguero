import { Component, NgZone, OnInit, OnDestroy } from '@angular/core';
import { User } from '../../models/user';
import { environment_development } from '../../../environments/environment.development';
import { interval, Subscription } from 'rxjs';
import { SolicitudAmistad } from '../../models/solicitud-amistad';
import { WebsocketService } from '../../services/websocket.service';
import { AuthService } from '../../services/auth.service';
import { ApiService } from '../../services/api.service';
import { Router, RouterModule } from '@angular/router';
import { ImageService } from '../../services/image.service';
import { FriendService } from '../../services/friend.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-amigos',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './amigos.component.html',
  styleUrl: './amigos.component.css'
})
export class AmigosComponent implements OnInit, OnDestroy {
  usuarios: User[] = [];
  usuariosFiltrados: User[] = [];
  amigos: User[] = [];
  amigosFiltrados: User[] = [];
  private BASE_URL = environment_development.apiUrl;
  onlineUserIds = new Set<number>();
  private subs: Subscription[] = [];
  terminoBusqueda: string = '';
  busquedaAmigos: string = '';
  usuarioApodo: string = '';
  usuarioFotoPerfil: string = '';
  usuarioId: number | null = null;
  perfil_default: string;
  solicitudesPendientes: SolicitudAmistad[] = [];
  errorMessage: string | null = null;

  constructor(
    public webSocketService: WebsocketService,
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router,
    private imageService: ImageService,
    private friendService: FriendService,
    private ngZone: NgZone
  ) {
    this.perfil_default = this.imageService.getImageUrl('Perfil_Deffault.png');
  }

  ngOnInit(): void {
    this.cargarInfoUsuario();
    this.obtenerUsuarios();
    this.cargarAmigos();
    this.obtenerSolicitudesPendientes();
    this.suscribirseAWebSockets();
    this.inicializarActualizaciones();
  }

  ngOnDestroy(): void {
    this.subs.forEach(sub => sub.unsubscribe());
  }

  private suscribirseAWebSockets(): void {
    this.subs.push(
      this.webSocketService.messageReceived.subscribe(message => {
        this.ngZone.run(() => this.procesarMensajesWebSocket(message));
      }),
      this.webSocketService.onlineUsers$.subscribe(users => {
        this.ngZone.run(() => {
          this.onlineUserIds = new Set(users);
          this.actualizarEstadosAmigos();
        });
      })
    );
  }

  private inicializarActualizaciones(): void {
    this.subs.push(
      interval(1000).subscribe(() => this.actualizarUsuariosConectados()),
      interval(30000).subscribe(() => this.actualizarListasCompletas())
    );
  }

  private procesarMensajesWebSocket(message: any): void {
    switch (message.type) {
      case 'friendRequest':
        this.manejarSolicitudAmistad(message);
        break;
      case 'friendRequestAccepted':
        this.manejarSolicitudAceptada(message);
        break;
      case 'friendRequestRejected':
        this.manejarSolicitudRechazada(message);
        break;
      case 'friendListUpdate':
        this.actualizarListasCompletas();
        break;
    }
  }

  private actualizarListasCompletas(): void {
    this.cargarAmigos();
    this.obtenerSolicitudesPendientes();
  }

  private actualizarEstadosAmigos(): void {
    this.amigos = this.amigos.map(amigo => ({
      ...amigo,
      UsuarioEstado: this.onlineUserIds.has(amigo.UserId) ? 'Conectado' : 'Desconectado'
    }));
    this.amigosFiltrados = [...this.amigos];
  }

  actualizarEstadoAmigo(friendId: number, estado: string) {
    const amigo = this.amigos.find(a => a.UserId === friendId);
    if (amigo) amigo.UserStatus = estado;
  }

  private actualizarUsuariosConectados(): void {
    this.webSocketService.fetchOnlineUsers().subscribe({
      next: (users) => this.onlineUserIds = new Set(users),
      error: (err) => console.error('Error actualizando usuarios:', err)
    });
  }

  private manejarSolicitudAmistad(message: any): void {
    const nuevaSolicitud: SolicitudAmistad = {
      friendshipId: message.requestId,
      userId: message.senderId,
      userNickname: message.senderName,
      userprofilephoto: this.validarUrlImagen(null)
    };
    this.solicitudesPendientes = [...this.solicitudesPendientes, nuevaSolicitud];
  }

  private manejarSolicitudAceptada(message: any): void {
    this.actualizarListasCompletas();
    this.errorMessage = `¡Ahora eres amigo de ${message.friendName}!`;
    setTimeout(() => this.errorMessage = null, 5000);
    this.actualizarListasCompletas();
  }

  private manejarSolicitudRechazada(message: any): void {
    this.solicitudesPendientes = this.solicitudesPendientes.filter(
      s => s.friendshipId !== message.requestId
    );
    this.errorMessage = message.reason || 'Solicitud rechazada';
    setTimeout(() => this.errorMessage = null, 5000);
    this.actualizarListasCompletas();
  }

  enviarSolicitud(receiverId: number): void {
    this.webSocketService.sendRxjs(JSON.stringify({
      type: 'sendFriendRequest',
      receiverId
    }));
    this.actualizarListasCompletas();
  }

  aceptarSolicitud(solicitud: SolicitudAmistad): void {
    this.webSocketService.sendRxjs(JSON.stringify({
      type: 'acceptFriendRequest',
      requestId: solicitud.friendshipId
    }));
    this.actualizarListasCompletas();
  }

  rechazarSolicitud(solicitud: SolicitudAmistad): void {
    this.webSocketService.sendRxjs(JSON.stringify({
      type: 'rejectFriendRequest',
      requestId: solicitud.friendshipId
    }));
    this.actualizarListasCompletas();
  }

  private cargarInfoUsuario(): void {
    const userInfo = this.authService.getUserData();
    if (userInfo) {
      this.usuarioApodo = userInfo.nickname;
      this.usuarioFotoPerfil = this.validarUrlImagen(userInfo.profilephoto);
      this.usuarioId = userInfo.id;
    } else {
      this.router.navigate(['/login']);
    }
  }

  obtenerUsuarios(): void {
    this.apiService.getUsuarios().subscribe(usuarios => {
      this.usuarios = usuarios
        .map(usuario => this.mapearUsuario(usuario))
        .filter(usuario => usuario.UserId !== this.usuarioId);
      this.usuariosFiltrados = [...this.usuarios];
    });
  }

  private cargarAmigos(): void {
    this.friendService.getFriendsList().subscribe({
      next: amigos => {
        this.amigos = amigos.map(a => this.mapearAmigo(a));
        this.actualizarEstadosAmigos();
      },
      error: err => console.error('Error cargando amigos:', err)
    });
  }

  private obtenerSolicitudesPendientes(): void {
    this.friendService.getPendingRequests().subscribe({
      next: solicitudes => {
        this.solicitudesPendientes = solicitudes.map(s => this.mapearSolicitud(s));
      },
      error: err => console.error('Error cargando solicitudes:', err)
    });
  }

  private mapearUsuario(usuario: any): User {
    return {
      UserId: usuario.userId,
      UserNickname: usuario.userNickname,
      UserProfilePhoto: this.validarUrlImagen(usuario.userProfilePhoto)
    };
  }

  private mapearAmigo(amigo: any): User {
    return {
      UserId: amigo.UsuarioId || amigo.userId,
      UserNickname: amigo.UsuarioApodo || amigo.userNickname,
      UserProfilePhoto: this.validarUrlImagen(amigo.UserProfilePhoto || amigo.userProfilePhoto),
      UserStatus: 'Desconectado'
    };
  }

  private mapearSolicitud(solicitud: any): SolicitudAmistad {
    return {
      friendshipId: solicitud.friendshipId,
      userId: solicitud.usuarioId,
      userNickname: solicitud.usuarioApodo,
      userprofilephoto: this.validarUrlImagen(solicitud.usuarioFotoPerfil)
    };
  }

  validarUrlImagen(fotoPerfil: string | null): string {
    return fotoPerfil ? `${this.BASE_URL}/fotos/${fotoPerfil}` : this.perfil_default;
  }

  buscarUsuarios(): void {
    this.usuariosFiltrados = this.terminoBusqueda.trim()
      ? this.usuarios.filter(usuario =>
          usuario.UserNickname?.toLowerCase().includes(this.terminoBusqueda.toLowerCase()) &&
          usuario.UserId !== this.usuarioId)
      : [...this.usuarios].filter(usuario => usuario.UserId !== this.usuarioId);
  }

  isUserOnline(userId: number): boolean {
    return this.onlineUserIds.has(userId);
  }

  logout(): void {
    this.webSocketService.disconnectRxjs();
    this.webSocketService.clearToken();
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
