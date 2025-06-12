
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

  private inicializarWebSockets(): void {
    const token = localStorage.getItem('accessToken');
    if (token) this.webSocketService.connectRxjs(token);
  }

  private cargarInfoUsuario(): void {
    const userInfo = this.authService.getUserData();
    if (userInfo) {
      this.userNickname = userInfo.nickname;
      this.userProfilePhoto = this.imageService.getImageUrl(userInfo.profilephoto);
      this.userId = userInfo.id;
    } else {
      this.router.navigate(['/login']);
    }
  }

  logout(): void {
    this.authService.logout();
    this.webSocketService.disconnect(); 
    this.router.navigate(['/login']);
  }
}