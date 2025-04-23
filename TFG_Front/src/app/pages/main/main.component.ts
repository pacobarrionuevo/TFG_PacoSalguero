import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-main',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent {
  isLoggedIn: boolean = false;
  isAdmin: boolean = false;
  constructor(private authService: AuthService, private router: Router) {}
  ngOnInit(): void {
    this.authService.isLoggedIn$.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
    });
    this.authService.isAdmin$.subscribe(admin => {
      this.isAdmin = admin;
    });
  }
  logout() {
    this.authService.logout();
    // ?    
    this.router.navigate(['/login']);
  }
}
