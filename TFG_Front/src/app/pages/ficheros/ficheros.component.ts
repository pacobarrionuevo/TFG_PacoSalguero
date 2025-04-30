import { Component, OnInit } from '@angular/core';
import { Service } from '../../models/service';
import { ServicesService } from '../../services/services.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-ficheros',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ficheros.component.html',
  styleUrls: ['./ficheros.component.css']
})
export class FicherosComponent implements OnInit {
  servicios: Service[] = [];

  nuevoServicio: Service = {
    nombre: '',
    abreviatura: '',
    color: ''
  };

  constructor(private servicioService: ServicesService) {}

  ngOnInit(): void {
    this.cargarServicios();
  }

  cargarServicios() {
    this.servicioService.getAll().subscribe({
      next: (data) => this.servicios = data,
      error: (err) => console.error('Error al obtener servicios:', err)
    });
  }

  crearServicio() {
    if (!this.nuevoServicio.nombre || !this.nuevoServicio.abreviatura || !this.nuevoServicio.color) {
      alert("Todos los campos son obligatorios");
      return;
    }

    this.servicioService.create(this.nuevoServicio).subscribe({
      next: (nuevo) => {
        this.servicios.push(nuevo);
        this.nuevoServicio = { nombre: '', abreviatura: '', color: '' };
      },
      error: (err) => {
        console.error('Error al crear servicio:', err);
        alert("Error al crear servicio");
      }
    });
  }
}
