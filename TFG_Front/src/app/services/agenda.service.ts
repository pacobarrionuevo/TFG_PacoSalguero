
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { EntradaAgenda } from '../models/entrada-agenda';

@Injectable({
  providedIn: 'root'
})
export class AgendaService {
  private apiUrl = 'https://localhost:7077/api/agenda'; 

  constructor(private http: HttpClient) { }

  getEntradas(): Observable<EntradaAgenda[]> {
  return this.http.get<any[]>(this.apiUrl).pipe(
    map(entradas => entradas.map(e => ({
      ...e,
      fecha: new Date(e.fecha), 
      hora: typeof e.hora === 'string' ? new Date(`1970-01-01T${e.hora}`) : e.hora
    })))
  );
}

  crearEntrada(entrada: EntradaAgenda): Observable<EntradaAgenda> {
    return this.http.post<EntradaAgenda>(this.apiUrl, entrada);
  }
}