import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';

import { User } from '../../models/user';

import { AuthService } from '../../services/auth.service';
import { AdminService } from '../../services/admin.service';
import { UserService } from '../../services/user.service';
import { ImageService } from '../../services/image.service';

interface DashboardStats {
  totalUsers: number;
  totalCustomers: number;
  totalServices: number;
  totalAgendaEntries: number;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  
  users: User[] = [];
  dashboardStats: DashboardStats | null = null;
  
  // Cache para manejar la edición
  private editUserCache: { [key: number]: User } = {};
  avatarFileCache: { [key: number]: File } = {};
  avatarPreviewCache: { [key: number]: string } = {};


  constructor(
    private authService: AuthService,
    private adminService: AdminService,
    private userService: UserService,
    public imageService: ImageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const userData = this.authService.getUserData();
    if (!userData || userData.role !== true) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadUsers();
    this.loadDashboardStats();
  }

  loadUsers(): void {
    this.userService.getUsuarios().subscribe(data => {
      this.users = data.map(u => ({ ...u, editing: false }));
    });
  }

  loadDashboardStats(): void {
    this.adminService.getDashboardStats().subscribe(stats => {
      this.dashboardStats = stats;
    });
  }

  // --- MÉTODOS DE EDICIÓN DE USUARIO ---
  
  editUser(user: User): void {
    // Guardar una copia del estado original
    this.editUserCache[user.userId!] = { ...user };
    user.editing = true;
  }

  cancelEdit(user: User): void {
    // Restaurar desde la caché
    Object.assign(user, this.editUserCache[user.userId!]);
    user.editing = false;
    delete this.editUserCache[user.userId!];
    delete this.avatarFileCache[user.userId!];
    delete this.avatarPreviewCache[user.userId!];
  }
  
  onAvatarSelected(event: Event, user: User): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.avatarFileCache[user.userId!] = file;
      
      const reader = new FileReader();
      reader.onload = () => {
        this.avatarPreviewCache[user.userId!] = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  saveUser(user: User): void {
    const updateObservables: any[] = [];
    
    // 1. Observable para datos de texto (si han cambiado)
    const originalUser = this.editUserCache[user.userId!];
    if (originalUser.userNickname !== user.userNickname || originalUser.userEmail !== user.userEmail) {
      updateObservables.push(this.adminService.updateUser(user.userId!, { userNickname: user.userNickname!, userEmail: user.userEmail! }));
    }

    // 2. Observable para el avatar (si se seleccionó uno nuevo)
    const newAvatarFile = this.avatarFileCache[user.userId!];
    if (newAvatarFile) {
      updateObservables.push(this.adminService.updateUserAvatar(user.userId!, newAvatarFile));
    }

    // Si no hay cambios, simplemente salimos del modo edición
    if (updateObservables.length === 0) {
      user.editing = false;
      return;
    }

    forkJoin(updateObservables).subscribe({
      next: (results) => {
        // Si se actualizó el avatar, el backend devuelve la nueva ruta
        const avatarResult = results.find(r => r && r.filePath);
        if (avatarResult) {
          user.userProfilePhoto = avatarResult.filePath;
        }
        
        alert('Usuario actualizado correctamente.');
        user.editing = false;
        delete this.editUserCache[user.userId!];
        delete this.avatarFileCache[user.userId!];
        delete this.avatarPreviewCache[user.userId!];
      },
      error: (err) => {
        console.error('Error al actualizar el usuario:', err);
        alert('No se pudo actualizar el usuario.');
        this.cancelEdit(user); // Revertir cambios en la UI
      }
    });
  }
  
  changeUserRole(user: User, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newRole = selectElement.value;
    
    if (user.userId && newRole) {
      this.adminService.changeUserRole(user.userId, newRole).subscribe({
        next: () => {
          user.role = newRole as 'user' | 'admin';
          alert('Rol actualizado correctamente.');
        },
        error: err => {
          console.error('Error al cambiar el rol:', err);
          alert('No se pudo actualizar el rol.');
          selectElement.value = user.role || 'user';
        }
      });
    }
  }

  deleteUser(userId: number | undefined): void {
    if (userId && window.confirm('¿Estás seguro de que quieres eliminar este usuario? Esta acción es irreversible.')) {
      this.adminService.deleteUser(userId).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.userId !== userId);
          alert('Usuario eliminado correctamente.');
          this.loadDashboardStats(); // Actualiza las estadísticas
        },
        error: err => {
          console.error('Error al eliminar usuario:', err);
          alert('No se pudo eliminar el usuario.');
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}