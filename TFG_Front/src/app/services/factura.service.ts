import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ServiceFacturado } from '../models/service-facturado';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las operaciones relacionadas con la facturación.
export class FacturaService {

  private apiUrl = environment_development.apiUrl;

  constructor(private http: HttpClient) { }

  // Envía un servicio facturado al backend.
  generarFacturaPDF(servicios: ServiceFacturado[]) {
    return this.http.post(`${this.apiUrl}/api/ServiceFacturado/generar-pdf`, servicios, {
      responseType: 'blob' // Importante para manejar el PDF como archivo
    });
  }
}