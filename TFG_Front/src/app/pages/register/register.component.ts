import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Image } from '../../models/image';
import { AuthService } from '../../services/auth.service';
import { ImageService } from '../../services/image.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  
  // Referencias a elementos del DOM (actualmente no se usan, podrían ser para diálogos modales).
  @ViewChild('addEditDialog')
  addOrEditDialog: ElementRef<HTMLDialogElement>;

  @ViewChild('removeDialog')
  deleteDialog: ElementRef<HTMLDialogElement>;

  images: Image[] = [];

  addOrEditForm: FormGroup;
  imageToEdit: Image = null;
  imageToDelete: Image = null;

  nickname: string = '';
  email: string = '';
  password: string = '';
  confirm_password: string = '';
  profile_photo: string = '';
  jwt: string | null = null;
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private imageService: ImageService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.jwt = localStorage.getItem('accessToken'); 

    // Inicializa el formulario reactivo para el registro.
    this.addOrEditForm = this.formBuilder.group({
      nickname: [''],
      email: [''],
      password: [''],
      confirm_password: [''],
      file: [null] // Campo para el archivo de imagen.
    });
  }

  // Captura el archivo seleccionado por el usuario y lo asigna al formulario.
  onFileSelected(event: any) {
    const image = event.target.files[0] as File;
    this.addOrEditForm.patchValue({ file: image });
  }

  // Envía los datos del formulario de registro al servicio de autenticación.
  async submit() {
    const file = this.addOrEditForm.get('file')?.value as File;
  
    if (!file) {
        alert("Por favor, selecciona una foto de perfil.");
        return;
    }

    // Construye un objeto FormData para enviar los datos, incluyendo el archivo.
    const formData = new FormData();
    formData.append('UserNickname', this.addOrEditForm.get('nickname')?.value);
    formData.append('UserEmail', this.addOrEditForm.get('email')?.value);
    formData.append('UserPassword', this.addOrEditForm.get('password')?.value);
    formData.append('UserConfirmPassword', this.addOrEditForm.get('confirm_password')?.value);
    formData.append('UserProfilePhoto', file);

    try {
        const result = await this.authService.register(formData).toPromise();
        if (result) {
            // Si el registro es exitoso, guarda el token y redirige.
            localStorage.setItem('accessToken', result.stringToken);
            this.jwt = result.stringToken;
            
            this.authService.updateAuthState();
            
            this.router.navigate(['/app/ficheros']);
        } else {
            console.error("No se recibió un token de acceso.");
        }
    } catch (error) {
        alert("Error al registrar: " + error.message);
    }
  }
}