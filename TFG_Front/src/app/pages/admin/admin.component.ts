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

// Interfaz para tipar los datos del dashboard.
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
  
  // Cachés para manejar la edición de usuarios de forma no destructiva.
  private editUserCache: { [key: number]: User } = {};
  avatarFileCache: { [key: number]: File } = {};
  avatarPreviewCache: { [key: number]: string } = {};


  constructor(
    private authService: AuthService,
    private adminService: AdminService,
    private userService: UserService,
    public imageService: ImageService, // Público para usarlo en la plantilla.
    private router: Router
  ) {}

  ngOnInit(): void {
    // Verifica si el usuario es administrador antes de cargar el componente.
    const userData = this.authService.getUserData();
    if (!userData || userData.role !== true) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadUsers();
    this.loadDashboardStats();
  }

  // Carga la lista de todos los usuarios.
  loadUsers(): void {
    this.userService.getUsuarios().subscribe(data => {
      this.users = data.map(u => ({ ...u, editing: false }));
    });
  }

  // Carga las estadísticas para el dashboard.
  loadDashboardStats(): void {
    this.adminService.getDashboardStats().subscribe(stats => {
      this.dashboardStats = stats;
    });
  }

  // --- MÉTODOS DE EDICIÓN DE USUARIO ---
  
  // Activa el modo de edición para un usuario y guarda su estado original.
  editUser(user: User): void {
    this.editUserCache[user.userId!] = { ...user };
    user.editing = true;
  }

  // Cancela la edición y restaura el estado original del usuario desde la caché.
  cancelEdit(user: User): void {
    Object.assign(user, this.editUserCache[user.userId!]);
    user.editing = false;
    delete this.editUserCache[user.userId!];
    delete this.avatarFileCache[user.userId!];
    delete this.avatarPreviewCache[user.userId!];
  }
  
  // Maneja la selección de un nuevo archivo de avatar.
  onAvatarSelected(event: Event, user: User): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.avatarFileCache[user.userId!] = file;
      
      // Genera una previsualización local de la imagen seleccionada.
      const reader = new FileReader();
      reader.onload = () => {
        this.avatarPreviewCache[user.userId!] = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  // Guarda los cambios del usuario (datos y/o avatar).
  saveUser(user: User): void {
    const updateObservables: any[] = [];
    
    // 1. Comprueba si los datos de texto han cambiado para añadirlos a la petición.
    const originalUser = this.editUserCache[user.userId!];
    if (originalUser.userNickname !== user.userNickname || originalUser.userEmail !== user.userEmail) {
      updateObservables.push(this.adminService.updateUser(user.userId!, { userNickname: user.userNickname!, userEmail: user.userEmail! }));
    }

    // 2. Comprueba si se ha seleccionado un nuevo avatar.
    const newAvatarFile = this.avatarFileCache[user.userId!];
    if (newAvatarFile) {
      updateObservables.push(this.adminService.updateUserAvatar(user.userId!, newAvatarFile));
    }

    // Si no hay cambios, simplemente sale del modo de edición.
    if (updateObservables.length === 0) {
      user.editing = false;
      return;
    }

    // Ejecuta todas las peticiones de actualización en paralelo.
    forkJoin(updateObservables).subscribe({
      next: (results) => {
        const avatarResult = results.find(r => r && r.filePath);
        if (avatarResult) {
          user.userProfilePhoto = avatarResult.filePath;
        }
        
        alert('Usuario actualizado correctamente.');
        user.editing = false;
        // Limpia las cachés de edición.
        delete this.editUserCache[user.userId!];
        delete this.avatarFileCache[user.userId!];
        delete this.avatarPreviewCache[user.userId!];
      },
      error: (err) => {
        console.error('Error al actualizar el usuario:', err);
        alert('No se pudo actualizar el usuario.');
        this.cancelEdit(user); // Revierte los cambios en la UI si hay un error.
      }
    });
  }
  
  // Cambia el rol de un usuario.
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
          // Revierte el cambio en el select si la petición falla.
          selectElement.value = user.role || 'user';
        }
      });
    }
  }

  // Elimina un usuario.
  deleteUser(userId: number | undefined): void {
    if (userId && window.confirm('¿Estás seguro de que quieres eliminar este usuario? Esta acción es irreversible.')) {
      this.adminService.deleteUser(userId).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.userId !== userId);
          alert('Usuario eliminado correctamente.');
          this.loadDashboardStats(); // Actualiza las estadísticas del dashboard.
        },
        error: err => {
          console.error('Error al eliminar usuario:', err);
          alert('No se pudo eliminar el usuario.');
        }
      });
    }
  }

  // Cierra la sesión del administrador.
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}