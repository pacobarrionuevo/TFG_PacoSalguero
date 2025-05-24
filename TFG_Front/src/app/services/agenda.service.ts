
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin, map, Observable, switchMap } from 'rxjs';
import { EntradaAgenda } from '../models/entrada-agenda';
import { ServicesService } from './services.service';

@Injectable({
  providedIn: 'root'
})
export class AgendaService {
  private apiUrl = 'https://localhost:7077/api/agenda'; 

  constructor(private http: HttpClient,
    private servicesService: ServicesService) { }

 getEntradas(): Observable<EntradaAgenda[]> {
    return this.http.get<EntradaAgenda[]>(this.apiUrl).pipe(
        switchMap(entradas => 
            this.servicesService.getAll().pipe(
                map(servicios => entradas.map(e => ({
                    ...e,
                    servicio: servicios.find(s => s.id === e.serviceId)
                })))
            )
        )
    );
}
  crearEntrada(entrada: EntradaAgenda): Observable<EntradaAgenda> {
    return this.http.post<EntradaAgenda>(this.apiUrl, entrada);
  }
}