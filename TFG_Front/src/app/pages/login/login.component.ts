import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthRequest } from '../../models/auth-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  emailOrNickname: string = '';
  password: string = '';
  remember: boolean = false;
  jwt: string | null = null;
  
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Carga las credenciales guardadas si el usuario marcó "Recuérdame".
    const savedAuthData = JSON.parse(localStorage.getItem('authData') || '{}');
    if (savedAuthData.recuerdame) {
      this.emailOrNickname = savedAuthData.emailOrNickname || '';
      this.password = savedAuthData.password || '';
      this.remember = savedAuthData.recuerdame || false;
    }
  }

  // Maneja el envío del formulario de login.
  async submit() {
    const authData: AuthRequest = { 
      UserEmailOrNickname: this.emailOrNickname, 
      UserPassword: this.password 
    };

    try {
      await this.authService.login(authData, this.remember).toPromise();

      // Guarda las credenciales si "Recuérdame" está activado.
      if (this.remember) {
        localStorage.setItem('authData', JSON.stringify({
          emailoapodo: this.emailOrNickname,
          contrasena: this.password,
          recuerdame: this.remember
        }));
      } else {
        localStorage.removeItem('authData');
      }

      // Redirige al usuario a la página principal de la aplicación.
      this.router.navigate(['/app/ficheros']);
    } catch (error: any) {
      // Manejo de errores específicos, como usuario baneado.
      if (error.status === 400 && error.error.message === "Usuario baneado") {
        alert("Usuario baneado: No puedes iniciar sesión.");
      } else {
        alert("Error al iniciar sesión");
      }
    }
  }
}