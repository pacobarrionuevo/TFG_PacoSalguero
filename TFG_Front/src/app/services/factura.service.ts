import { Injectable } from '@angular/core';
import { environment_development } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ServiceFacturado } from '../models/service-facturado';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
// Servicio para gestionar las operaciones relacionadas con la facturación.
export class FacturaService {

  private apiUrl = environment_development.apiUrl;

  constructor(private http: HttpClient) { }

  // Envía un servicio facturado al backend.
  generarFacturaPDF(serviciosSeleccionados: any[]): Observable<{ url: string }> {
  return this.http.post<{ url: string }>(
    `${this.apiUrl}/api/ServiceFacturado/generar-pdf`,
    serviciosSeleccionados
  );
}
}