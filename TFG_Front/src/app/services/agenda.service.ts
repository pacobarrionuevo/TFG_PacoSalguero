import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, map, Observable, switchMap } from 'rxjs';
import { EntradaAgenda } from '../models/entrada-agenda';
import { ServicesService } from './services.service';
import { environment_development } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AgendaService {
  private apiUrl = `${environment_development.apiUrl}/api/agenda`; 

  constructor(private http: HttpClient,
    private servicesService: ServicesService) { }

  // Obtiene todas las entradas de la agenda y enriquece cada una con la información completa del servicio.
  getEntradas(): Observable<EntradaAgenda[]> {
    return this.http.get<EntradaAgenda[]>(this.apiUrl).pipe(
        // Usa switchMap para encadenar la obtención de entradas con la de servicios.
        switchMap(entradas => 
            this.servicesService.getAll().pipe(
                // Usa map para combinar los resultados y añadir el objeto 'servicio' a cada entrada.
                map(servicios => entradas.map(e => ({
                    ...e,
                    servicio: servicios.find(s => s.id === e.serviceId)
                })))
            )
        )
    );
  }

  // Crea una nueva entrada en la agenda.
  crearEntrada(entrada: EntradaAgenda): Observable<EntradaAgenda> {
    return this.http.post<EntradaAgenda>(this.apiUrl, entrada);
  }

  // Obtiene las entradas de un mes y año específicos.
  getEntradasPorMes(year: number, month: number): Observable<EntradaAgenda[]> {
    return this.http.get<EntradaAgenda[]>(`${this.apiUrl}/mes/${year}/${month}`);
  }
}