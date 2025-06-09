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
    const savedAuthData = JSON.parse(localStorage.getItem('authData') || '{}');
    if (savedAuthData.recuerdame) {
      this.emailOrNickname = savedAuthData.emailOrNickname || '';
      this.password = savedAuthData.password || '';
      this.remember = savedAuthData.recuerdame || false;
    }
  }

  async submit() {
    const authData: AuthRequest = { 
      UserEmailOrNickname: this.emailOrNickname, 
      UserPassword: this.password 
    };

    try {
      await this.authService.login(authData, this.remember).toPromise();

      if (this.remember) {
        localStorage.setItem('authData', JSON.stringify({
          emailoapodo: this.emailOrNickname,
          contrasena: this.password,
          recuerdame: this.remember
        }));
      } else {
        localStorage.removeItem('authData');
      }

      this.router.navigate(['/app/ficheros']);
    } catch (error: any) {
      if (error.status === 400 && error.error.message === "Usuario baneado") {
        alert("Usuario baneado: No puedes iniciar sesión.");
      } else {
        alert("Error al iniciar sesión");
      }
    }
  }
}
