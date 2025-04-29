import { Component, OnInit } from '@angular/core';
import { AgendaService } from '../../services/agenda.service';
import { EntradaAgenda } from '../../models/entrada-agenda';

@Component({
  selector: 'app-menu-agenda',
  templateUrl: './menu-agenda.component.html',
  styleUrls: ['./menu-agenda.component.css']
})
export class ListaAgendaComponent implements OnInit {
  entradas: EntradaAgenda[] = [];

  constructor(private agendaService: AgendaService) { }

  ngOnInit(): void {
    this.cargarEntradas();
  }

  cargarEntradas(): void {
    this.agendaService.getEntradas().subscribe({
      next: (entradas) => this.entradas = entradas,
      error: (err) => console.error('Error al cargar entradas:', err)
    });
  }
}