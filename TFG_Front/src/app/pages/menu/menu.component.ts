import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ImageService } from '../../services/image.service';
import { AuthService } from '../../services/auth.service';
import { environment_development } from '../../../environments/environment.development';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css'
})
export class MenuComponent implements OnInit{
  userProfilePhoto: string = '';
  userNickname: string = '';
  userId: number | null = null;

  private BASE_URL = environment_development.apiUrl;

  constructor(private imageService: ImageService, private authService: AuthService, private router: Router,) {}

  ngOnInit(): void {
      this.cargarInfoUsuario();
  }

  private cargarInfoUsuario(): void {
    const userInfo = this.authService.getUserData();
    if (userInfo) {
      this.userNickname = userInfo.nickname;
      this.userProfilePhoto = this.validarUrlImagen(userInfo.profilephoto);
      this.userId = userInfo.id;
    } else {
      this.router.navigate(['/login']);
    }
  }

  validarUrlImagen(fotoPerfil: string | null): string {
    return fotoPerfil ? `${this.BASE_URL}/fotos/${fotoPerfil}` : ''
  }
}
