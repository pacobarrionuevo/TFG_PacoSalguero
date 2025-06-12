import { Component, OnDestroy, OnInit, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
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
  // Almacena los IDs de los usuarios a los que se ha enviado una solicitud para evitar duplicados.
  solicitudesEnviadas: number[] = [];
  
  private subscriptions: Subscription = new Subscription();
  private refreshTimer: any;

  constructor(
    private friendService: FriendService,
    private userService: UserService,
    private authService: AuthService,
    private websocketService: WebsocketService,
    public imageService: ImageService,
    // NgZone se usa para asegurar que las actualizaciones de WebSocket se reflejen en la UI de Angular.
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    console.log('[Amigos Component] ngOnInit: Iniciando componente.');
    this.usuarioId = this.authService.getUserData()?.id ?? null;
    
    this.cargarDatosIniciales();
    this.suscribirAEventosWebSocket();

    // Refresca periódicamente los datos para mantener la consistencia.
    this.refreshTimer = setInterval(() => {
      console.log('[Amigos Component] Refrescando datos de amigos y solicitudes...');
      this.cargarAmigos();
      this.cargarSolicitudes();
    }, 1000); // Refresca cada segundo.
  }

  ngOnDestroy(): void {
    console.log('[Amigos Component] ngOnDestroy: Limpiando suscripciones y temporizador.');
    this.subscriptions.unsubscribe();
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
      // Compara si es necesario actualizar para evitar parpadeos innecesarios en la UI.
      if (JSON.stringify(this.amigosFiltrados) !== JSON.stringify(amigos)) {
        this.amigosFiltrados = amigos;
      }
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

  // Se suscribe a los eventos del WebSocket para recibir actualizaciones en tiempo real.
  private suscribirAEventosWebSocket(): void {
    console.log('[Amigos Component] Suscribiendo a eventos de WebSocket...');
    
    // Suscripción a cambios de estado de amigos (online/offline).
    const statusSub = this.websocketService.friendStatusUpdate$.subscribe(update => {
      this.ngZone.run(() => {
        const amigoIndex = this.amigosFiltrados.findIndex(a => a.userId === update.userId);
        if (amigoIndex !== -1) {
          this.amigosFiltrados[amigoIndex].isOnline = update.isOnline;
          this.amigosFiltrados[amigoIndex].lastSeen = update.lastSeen;
          this.amigosFiltrados = [...this.amigosFiltrados]; // Crea una nueva referencia para forzar la detección de cambios.
        }
      });
    });

    // Suscripción a notificaciones de nueva amistad (cuando se acepta una solicitud).
    const newFriendSub = this.websocketService.newFriendNotification$.subscribe(nuevoAmigo => {
      this.ngZone.run(() => {
        if (!this.amigosFiltrados.some(a => a.userId === nuevoAmigo.userId)) {
            this.amigosFiltrados = [...this.amigosFiltrados, nuevoAmigo];
        }
        // Elimina la solicitud de la lista de pendientes.
        this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.userId !== nuevoAmigo.userId);
      });
    });

    // Suscripción a nuevas solicitudes de amistad recibidas.
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

  // Envía un mensaje por WebSocket para aceptar una solicitud.
  aceptarSolicitud(solicitud: SolicitudAmistad): void {
    this.websocketService.send(JSON.stringify({
      type: 'acceptFriendRequest',
      requestId: solicitud.friendshipId
    }));
  }

  // Envía una petición HTTP para rechazar una solicitud.
  rechazarSolicitud(solicitud: SolicitudAmistad): void {
    this.friendService.rechazarSolicitud(solicitud.friendshipId).subscribe(() => {
      this.solicitudesPendientes = this.solicitudesPendientes.filter(s => s.friendshipId !== solicitud.friendshipId);
    });
  }

  // Envía una petición HTTP para crear la solicitud de amistad.
  enviarSolicitud(receiverId: number | undefined): void {
    if (receiverId === undefined) return;

    this.friendService.sendFriendRequest(receiverId).subscribe({
      next: (response) => {
        console.log('Solicitud de amistad enviada con éxito', response);
        // Marcamos la solicitud como enviada para actualizar la UI
        this.solicitudesEnviadas.push(receiverId);
        alert('Solicitud de amistad enviada.');
      },
      error: (err) => {
        console.error('Error al enviar la solicitud de amistad:', err);
        alert('No se pudo enviar la solicitud de amistad.');
      }
    });
  }

  // Filtra la lista de usuarios según el término de búsqueda.
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

  // Calcula y formatea el texto del estado de conexión de un amigo.
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
    
    if (seconds < 60) {
      return 'Hace un momento';
    }
    
    const intervals: { [key: string]: number } = {
      año: 31536000, mes: 2592000, día: 86400, hora: 3600, minuto: 60
    };

    for (const part in intervals) {
      const interval = Math.floor(seconds / intervals[part]);
      if (interval >= 1) {
        const plural = interval > 1 ? (part === 'mes' ? 'es' : 's') : '';
        return `Hace ${interval} ${part}${plural}`;
      }
    }
    
    return 'Hace un momento';
  }
}