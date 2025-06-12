import { Component, OnInit } from '@angular/core';
import { EntradaAgenda } from '../../models/entrada-agenda';
import { AgendaService } from '../../services/agenda.service';
import { DatePipe } from '@angular/common';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ServicesService } from '../../services/services.service';
import { Service } from '../../models/service';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css',
  providers: [DatePipe]
})
export class CalendarComponent implements OnInit {
  currentDate: Date = new Date();
  weeks: Date[][] = [];
  entradasMes: EntradaAgenda[] = [];
  servicios: Service[] = [];

  constructor(
    private agendaService: AgendaService,
    private datePipe: DatePipe,
    private servicesService: ServicesService,
  ) { }

  ngOnInit(): void {
    // Carga primero los servicios para poder asociarlos a las entradas de la agenda.
    this.cargarServicios();
    this.generarCalendario();
    this.cargarEntradasMes();
  }

  cargarServicios(): void {
    this.servicesService.getAll().subscribe({
      next: (servicios) => {
        this.servicios = servicios;
      },
      error: (err) => console.error('Error al cargar servicios:', err)
    });
  }

  // Carga las entradas de la agenda para el mes y año actualmente seleccionados.
  cargarEntradasMes(): void {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth() + 1;

    this.agendaService.getEntradasPorMes(year, month).subscribe({
      next: (entradas) => {
        // Mapea las entradas para asociarles su servicio correspondiente y convertir la fecha.
        this.entradasMes = entradas.map(e => {
          const servicio = this.servicios.find(s => s.id === e.serviceId);
          const fecha = new Date(e.fechaHora);
          return {
            ...e,
            fechaHora: fecha,
            servicio: servicio
          };
        });
      }
    });
  }

  // Genera la estructura de semanas y días para el mes actual.
  generarCalendario(): void {
    const start = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), 1);
    const end = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 0);
    
    // Ajusta la fecha de inicio al lunes de la primera semana.
    start.setDate(start.getDate() - (start.getDay() || 7) + 1);
    
    // Ajusta la fecha de fin al domingo de la última semana.
    end.setDate(end.getDate() + (7 - end.getDay()));

    this.weeks = [];
    let current = new Date(start);
    
    while (current <= end) {
      const week: Date[] = [];
      for (let i = 0; i < 7; i++) {
        week.push(new Date(current));
        current.setDate(current.getDate() + 1);
      }
      this.weeks.push(week);
    }
  }

  // Cambia al mes anterior o siguiente y recarga los datos.
  cambiarMes(delta: number): void {
    this.currentDate.setMonth(this.currentDate.getMonth() + delta);
    this.currentDate = new Date(this.currentDate);
    this.generarCalendario();
    this.cargarEntradasMes();
  }

  // Filtra y devuelve las entradas de la agenda para un día específico.
  entradasDelDia(fecha: Date): EntradaAgenda[] {
    return this.entradasMes.filter(e => {
      const entradaDate = new Date(e.fechaHora);
      return (
        entradaDate.getFullYear() === fecha.getFullYear() &&
        entradaDate.getMonth() === fecha.getMonth() &&
        entradaDate.getDate() === fecha.getDate()
      );
    });
  }

  // Comprueba si un día pertenece al mes que se está visualizando.
  esMesActual(fecha: Date): boolean {
    return fecha.getMonth() === this.currentDate.getMonth();
  }
}