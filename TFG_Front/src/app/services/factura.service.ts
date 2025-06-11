import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ServiceFacturado } from '../models/service-facturado';

@Injectable({
  providedIn: 'root'
})
export class FacturaService {

  private apiUrl = environment_development.apiUrl;

  constructor(private http: HttpClient) { }

  enviarServicioFacturado(servicio: ServiceFacturado) {
    return this.http.post(this.apiUrl, servicio);
  }
}
