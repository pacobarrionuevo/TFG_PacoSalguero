import { Component, OnInit } from '@angular/core';
import { AgendaService } from '../../services/agenda.service';
import { EntradaAgenda } from '../../models/entrada-agenda';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
@Component({
  selector: 'app-agenda',
  standalone: true,
  imports: [CommonModule,RouterModule],
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
      next: (data) => {
        this.entradas = data.map(entrada => {
          return {
            ...entrada,
            fecha: new Date(entrada.fecha) 
          };
        });
      },
      error: (err) => console.error('Error al cargar entradas:', err)
    });
  }
}