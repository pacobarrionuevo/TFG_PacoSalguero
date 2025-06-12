import { Component } from '@angular/core';
import { EntradaAgenda } from '../../models/entrada-agenda';
import { AgendaService } from '../../services/agenda.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ServiceFacturado } from '../../models/service-facturado';

@Component({
  selector: 'app-facturas',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './facturas.component.html',
  styleUrl: './facturas.component.css'
})
export class FacturasComponent {
  
  entradas: EntradaAgenda[] = [];
  serviciosSeleccionados: ServiceFacturado[] = [];

  constructor(private agendaService: AgendaService) { }

  ngOnInit(): void {
    this.cargarEntradas();
  }

  // Carga las entradas de la agenda y añade la propiedad 'seleccionado' para el control en la UI.
  cargarEntradas(): void {
    this.agendaService.getEntradas().subscribe({
      next: (data) => {
        this.entradas = data.map(entrada => ({
          ...entrada,
          fecha: new Date(entrada.fechaHora),
          seleccionado: false
        }));
      },
      error: (err) => console.error('Error al cargar entradas:', err)
    });
  }

  // Comprueba si hay al menos una entrada seleccionada para habilitar el botón de facturar.
  haySeleccionados(): boolean {
    return this.entradas.some(e => e.seleccionado);
  }

  // Filtra las entradas seleccionadas y las mapea al modelo ServiceFacturado.
  facturarSeleccionados(): void {
    this.serviciosSeleccionados = this.entradas
      .filter(e => e.seleccionado)
      .map(e => ({
        centro: e.centroTrabajo,
        cliente: e.cliente,
        fecha: e.fechaHora,
        paciente: e.paciente,
        servicio: e.servicio?.nombre || '(Sin nombre)',
        observaciones: e.observaciones
      }));

    // Aquí iría la lógica para enviar `serviciosSeleccionados` al backend para generar la factura.
    console.log('Datos simplificados para facturar:', this.serviciosSeleccionados);
  }
}