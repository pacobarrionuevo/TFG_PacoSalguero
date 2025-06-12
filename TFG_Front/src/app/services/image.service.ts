import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { Image } from '../models/image';
import { CreateOrUpdateImageRequest } from '../models/create-update-image-request';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las operaciones relacionadas con imágenes.
export class ImageService {

  private baseURL = environment_development.apiUrl; 

  constructor(private api: ApiService) { }

  // Construye la URL completa para una imagen a partir de su ruta relativa.
  getImageUrl(imagePath: string | undefined | null): string {
    if (!imagePath) {
      // Devuelve una imagen por defecto si la ruta no está definida.
      return 'assets/img/perfil_default.png'; 
    }

    // Lógica para manejar rutas de fotos de perfil que no incluyen el prefijo de la carpeta.
    if (!imagePath.includes('/')) {
      imagePath = `fotos/${imagePath}`;
    }

    return `${this.baseURL}/${imagePath}`;
  }

  // Los siguientes métodos son para un CRUD de imágenes genérico (actualmente no se usa en toda su capacidad).
  getAllImages(): Promise<Result<Image[]>> {
    return this.api.get<Image[]>('images');
  }

  addImage(createOrUpdateImageRequest: CreateOrUpdateImageRequest): Promise<Result<Image>> {
    const formData = new FormData();
    formData.append('name', createOrUpdateImageRequest.name);
    formData.append('file', createOrUpdateImageRequest.file);
    return this.api.post<Image>('images', formData);
  }

  updateImage(id: number, createOrUpdateImageRequest: CreateOrUpdateImageRequest): Promise<Result> {
    const formData = new FormData();
    formData.append('name', createOrUpdateImageRequest.name);
    formData.append('file', createOrUpdateImageRequest.file);
    return this.api.put(`images/${id}`, formData);
  }

  deleteImage(id: number): Promise<Result> {
    return this.api.delete(`images/${id}`);
  }
}