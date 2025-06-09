import { Component } from '@angular/core';
import { EntradaAgenda } from '../../models/entrada-agenda';
import { AgendaService } from '../../services/agenda.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-facturas',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './facturas.component.html',
  styleUrl: './facturas.component.css'
})
export class FacturasComponent {
  
  entradas: EntradaAgenda[] = [];
  serviciosSeleccionados: EntradaAgenda[] = [];

  constructor(private agendaService: AgendaService) { }

  ngOnInit(): void {
    this.cargarEntradas();
  }

  cargarEntradas(): void {
    this.agendaService.getEntradas().subscribe({
      next: (data) => {
        this.entradas = data.map(entrada => ({
          ...entrada,
          fecha: new Date(entrada.fechaHora),
          seleccionado: false // <-- se añade esta propiedad
        }));
      },
      error: (err) => console.error('Error al cargar entradas:', err)
    });
  }

  haySeleccionados(): boolean {
    return this.entradas.some(e => e.seleccionado);
  }

  facturarSeleccionados(): void {
    this.serviciosSeleccionados = this.entradas.filter(e => e.seleccionado);
    console.log('Servicios seleccionados para facturar:', this.serviciosSeleccionados);
  }
}
