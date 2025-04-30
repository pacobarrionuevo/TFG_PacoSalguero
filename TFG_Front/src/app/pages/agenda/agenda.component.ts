import { Component, OnInit } from '@angular/core';
import { AgendaService } from '../../services/agenda.service';
import { EntradaAgenda } from '../../models/entrada-agenda';
@Component({
  selector: 'app-agenda',
  standalone: true,
  imports: [],
  templateUrl: './agenda.component.html',
  styleUrl: './agenda.component.css'
})
export class AgendaComponent {
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
