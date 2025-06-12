import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Result } from '../models/result';
import { Image } from '../models/image';
import { environment_development } from '../../environments/environment.development';
import { CreateOrUpdateImageRequest } from '../models/create-update-image-request';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  private baseURL = `${environment_development.apiUrl}/fotos`; 

  constructor(private api: ApiService) { }

  getImageUrl(imageName: string): string {
    if (!imageName) {
      return 'assets/img/perfil_default.png'; 
    }
    return `${this.baseURL}/${imageName}`;
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