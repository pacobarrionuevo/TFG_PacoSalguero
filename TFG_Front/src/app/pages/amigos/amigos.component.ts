import { Component, OnDestroy, OnInit, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription, interval } from 'rxjs'; // Importar interval
import { User } from '../../models/user';
import { SolicitudAmistad } from '../../models/solicitud-amistad';
import { FriendService } from '../../services/friend.service';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';
import { ImageService } from '../../services/image.service';

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
  
  private subscriptions: Subscription = new Subscription();
  private refreshTimer: any; // Variable para el temporizador

  constructor(
    private friendService: FriendService,
    private userService: UserService,
    private authService: AuthService,
    private websocketService: WebsocketService,
    public imageService: ImageService,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    console.log('[Amigos Component] ngOnInit: Iniciando componente.');
    this.usuarioId = this.authService.getUserData()?.id ?? null;
    
    // Carga inicial de datos
    this.cargarDatosIniciales();
    
    // Suscripción a eventos WebSocket
    this.suscribirAEventosWebSocket();

    // --- IMPLEMENTACIÓN DE LA IDEA DE REFRESCO ---
    // Iniciar un temporizador que recargue los datos cada 10 segundos (10000 ms)
    this.refreshTimer = setInterval(() => {
      console.log('[Amigos Component] Refrescando datos de amigos y solicitudes...');
      this.cargarAmigos();
      this.cargarSolicitudes();
    }, 10000);
  }

  ngOnDestroy(): void {
    console.log('[Amigos Component] ngOnDestroy: Limpiando suscripciones y temporizador.');
    this.subscriptions.unsubscribe();
    // Limpiar el temporizador para evitar fugas de memoria
    if (this.refreshTimer) {
      clearInterval(this.refreshTimer);
    }
  }

  cargarDatosIniciales(): void {
    this.cargarAmigos();
    this.cargarSolicitudes();
    this.cargarTodosLosUsuarios();
  }

  cargarAmigos(): void {
    this.friendService.getFriendsList().subscribe(amigos => {
      this.amigosFiltrados = amigos;
    });
  }

  cargarSolicitudes(): void {
    this.friendService.getPendingRequests().subscribe(solicitudes => {
      this.solicitudesPendientes = solicitudes;
    });
  }

  cargarTodosLosUsuarios(): void {
    this.userService.getUsuarios().subscribe(usuarios => {
      this.todosLosUsuarios = usuarios;
      this.usuariosFiltrados = this.todosLosUsuarios; 
    });
  }

  private suscribirAEventosWebSocket(): void {
    console.log('[Amigos Component] Suscribiendo a eventos de WebSocket...');
    
    const statusSub = this.websocketService.friendStatusUpdate$.subscribe(update => {
      this.ngZone.run(() => {
        const amigoIndex = this.amigosFiltrados.findIndex(a => a.userId === update.userId);
        if (amigoIndex !== -1) {
          this.amigosFiltrados[amigoIndex] = { ...this.amigosFiltrados[amigoIndex], isOnline: update.isOnline, lastSeen: update.lastSeen };
          this.amigosFiltrados = [...this.amigosFiltrados];
        }
      });
    });

    const newFriendSub = this.websocketService.newFriendNotification$.subscribe(nuevoAmigo => {
      this.ngZone.run(() => {
        if (!this.amigosFiltrados.some(a => a.userId === nuevoAmigo.userId)) {
            this.amigosFiltrados = [...this.amigosFiltrados, nuevoAmigo];
        }
        this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.userId !== nuevoAmigo.userId);
      });
    });

    const newRequestSub = this.websocketService.newFriendRequest$.subscribe(nuevaSolicitud => {
      this.ngZone.run(() => {
        if (!this.solicitudesPendientes.some(s => s.friendshipId === nuevaSolicitud.friendshipId)) {
          this.solicitudesPendientes = [...this.solicitudesPendientes, nuevaSolicitud];
        }
      });
    });

    this.subscriptions.add(statusSub);
    this.subscriptions.add(newFriendSub);
    this.subscriptions.add(newRequestSub);
  }

  aceptarSolicitud(solicitud: SolicitudAmistad): void {
    this.websocketService.send(JSON.stringify({
      type: 'acceptFriendRequest',
      requestId: solicitud.friendshipId
    }));
  }

  rechazarSolicitud(solicitud: SolicitudAmistad): void {
    this.friendService.rechazarSolicitud(solicitud.friendshipId).subscribe(() => {
      this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.friendshipId !== solicitud.friendshipId);
    });
  }

  enviarSolicitud(receiverId: number | undefined): void {
    if (receiverId === undefined) return;
    this.websocketService.send(JSON.stringify({
      type: 'sendFriendRequest',
      receiverId: receiverId
    }));
    this.solicitudesEnviadas.push(receiverId);
  }

  buscarUsuarios(): void {
    if (!this.terminoBusqueda) {
      this.usuariosFiltrados = this.todosLosUsuarios;
      return;
    }
    this.usuariosFiltrados = this.todosLosUsuarios.filter(u =>
      u.userNickname?.toLowerCase().includes(this.terminoBusqueda.toLowerCase())
    );
  }

  esAmigo(id: number | undefined): boolean {
    if (id === undefined) return false;
    return this.amigosFiltrados.some(a => a.userId === id);
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
    let interval = seconds / 3600;
    if (interval > 1) return `Hace ${Math.floor(interval)} horas`;
    interval = seconds / 60;
    if (interval > 1) return `Hace ${Math.floor(interval)} minutos`;
    return 'Hace un momento';
  }
}