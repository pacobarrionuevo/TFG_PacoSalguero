
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EntradaAgenda } from '../models/entrada-agenda';

@Injectable({
  providedIn: 'root'
})
export class AgendaService {
  private apiUrl = 'https://localhost:7077/api/agenda'; 

  constructor(private http: HttpClient) { }

  getEntradas(): Observable<EntradaAgenda[]> {
    return this.http.get<EntradaAgenda[]>(this.apiUrl);
  }

  crearEntrada(entrada: EntradaAgenda): Observable<EntradaAgenda> {
    return this.http.post<EntradaAgenda>(this.apiUrl, entrada);
  }
}