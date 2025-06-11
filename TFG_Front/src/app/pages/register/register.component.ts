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

    this.addOrEditForm = this.formBuilder.group({
      nickname: [''],
      email: [''],
      password: [''],
      confirm_password: [''],
      file: [null]
    });
  }

  openDialog(dialogRef: ElementRef<HTMLDialogElement>) {
    dialogRef.nativeElement.showModal();
  }

  closeDialog(dialogRef: ElementRef<HTMLDialogElement>) {
    dialogRef.nativeElement.close();
  }

  onFileSelected(event: any) {
    const image = event.target.files[0] as File;
    this.addOrEditForm.patchValue({ file: image });
  }

  async upateImageList() {
    const request = await this.imageService.getAllImages();

    if (request.success) {
      this.images = request.data;
    }
  }

  async submit() {
    const file = this.addOrEditForm.get('file')?.value as File;
  
    if (!file) {
        alert("Por favor, selecciona una foto de perfil.");
        return;
    }

    const formData = new FormData();
    formData.append('UserNickname', this.addOrEditForm.get('nickname')?.value);
    formData.append('UserEmail', this.addOrEditForm.get('email')?.value);
    formData.append('UserPassword', this.addOrEditForm.get('password')?.value);
    formData.append('UserConfirmPassword', this.addOrEditForm.get('confirm_password')?.value);
    formData.append('UserProfilePhoto', file);
    console.log(formData);

    try {
        const result = await this.authService.register(formData).toPromise();
        if (result) {
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
