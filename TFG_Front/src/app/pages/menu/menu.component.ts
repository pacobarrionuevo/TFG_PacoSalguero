import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ImageService } from '../../services/image.service';
import { AuthService } from '../../services/auth.service';
import { WebsocketService } from '../../services/websocket.service';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent implements OnInit {
  userProfilePhoto: string = '';
  userNickname: string = '';
  userId: number | null = null;

  constructor(
    private imageService: ImageService,
    private authService: AuthService,
    public webSocketService: WebsocketService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarInfoUsuario();
    this.inicializarWebSockets();
  }

  // Inicia la conexión WebSocket si hay un token disponible.
  private inicializarWebSockets(): void {
    const token = localStorage.getItem('accessToken');
    if (token) this.webSocketService.connectRxjs(token);
  }

  // Carga la información del usuario desde el token JWT para mostrarla en el menú.
  private cargarInfoUsuario(): void {
    const userInfo = this.authService.getUserData();
    if (userInfo) {
      this.userNickname = userInfo.nickname;
      this.userProfilePhoto = this.imageService.getImageUrl(userInfo.profilephoto);
      this.userId = userInfo.id;
    } else {
      // Si no hay información de usuario, redirige al login.
      this.router.navigate(['/login']);
    }
  }

  // Cierra la sesión, desconecta el WebSocket y redirige al login.
  logout(): void {
    this.authService.logout();
    this.webSocketService.disconnect(); 
    this.router.navigate(['/login']);
  }
}