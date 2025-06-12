// TFG_Front/src/app/services/image.service.ts

import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { Image } from '../models/image';
import { CreateOrUpdateImageRequest } from '../models/create-update-image-request';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  private baseURL = environment_development.apiUrl; 

  constructor(private api: ApiService) { }

  getImageUrl(imagePath: string | undefined | null): string {
    if (!imagePath) {
      // Imagen por defecto si no hay ruta
      return 'assets/img/perfil_default.png'; 
    }

   
    // Si la ruta no contiene una barra, asumimos que es una foto de perfil
    // y le añadimos el prefijo "fotos/".
    if (!imagePath.includes('/')) {
      imagePath = `fotos/${imagePath}`;
    }

    return `${this.baseURL}/${imagePath}`;
  }

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