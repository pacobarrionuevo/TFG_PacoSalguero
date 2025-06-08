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

  cargarEntradasMes(): void {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth() + 1;

    this.agendaService.getEntradasPorMes(year, month).subscribe({
      next: (entradas) => {
        this.entradasMes = entradas.map(e => {
          const servicio = this.servicios.find(s => s.id === e.serviceId);
          return {
            ...e,
            fechaHora: new Date(e.fechaHora),
            servicio: servicio
          };
        });
        console.log('Entradas procesadas:', this.entradasMes);
      }
    });
  }

  generarCalendario(): void {
    const start = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth(), 1);
    const end = new Date(this.currentDate.getFullYear(), this.currentDate.getMonth() + 1, 0);
    
    // Ajustar inicio a lunes
    start.setDate(start.getDate() - (start.getDay() || 7) + 1);
    
    // Ajustar fin a domingo
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

  cambiarMes(delta: number): void {
    this.currentDate.setMonth(this.currentDate.getMonth() + delta);
    this.currentDate = new Date(this.currentDate); // Forzar actualización
    this.generarCalendario();
    this.cargarEntradasMes();
  }

  entradasDelDia(fecha: Date): EntradaAgenda[] {
    return this.entradasMes.filter(e => 
        e.fechaHora.getFullYear() === fecha.getFullYear() &&
        e.fechaHora.getMonth() === fecha.getMonth() &&
        e.fechaHora.getDate() === fecha.getDate()
    );
}

  esMesActual(fecha: Date): boolean {
    return fecha.getMonth() === this.currentDate.getMonth();
  }
}
