import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ImageService } from '../../services/image.service';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent {
  enfermeras: string;
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;

  constructor(private imageService: ImageService, private authService: AuthService, private router: Router) {
    // Obtiene la URL de la imagen a través del servicio de imágenes.
    this.enfermeras = this.imageService.getImageUrl('images/enfermeras.jpg');
  }

  ngOnInit(): void {
    // Se suscribe a los observables del servicio de autenticación para reaccionar a los cambios de estado.
    this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
    });
    this.authService.isAdmin$.subscribe(admin => {
      this.isAdmin = admin;
    });
  }
  
  // Cierra la sesión del usuario y lo redirige a la página de login.
  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}