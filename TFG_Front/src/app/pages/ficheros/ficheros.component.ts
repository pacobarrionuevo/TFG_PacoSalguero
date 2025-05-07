import { Component, OnInit } from '@angular/core';
import { Service } from '../../models/service';
import { ServicesService } from '../../services/services.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentMethod } from '../../models/payment-method';
import { PaymentMethodService } from '../../services/payment-method.service';

@Component({
  selector: 'app-ficheros',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ficheros.component.html',
  styleUrls: ['./ficheros.component.css']
})
export class FicherosComponent implements OnInit {
  servicios: Service[] = [];
  paymentMethods: PaymentMethod[] = [];

  nuevoServicio: Service = {
    nombre: '',
    abreviatura: '',
    color: ''
  };

  newPaymentMethod: PaymentMethod = {
    Method: '',
    Installments: 0,
    FirstPaymentDays: 0,
    DaysBetweenPayments: 0
  }

  constructor(private servicioService: ServicesService, private paymentMethodService: PaymentMethodService) {}

  ngOnInit(): void {
    this.cargarServicios();
    this.cargarMetodosPago();
  }

  ///////////////////////////////////////////////////
  ////// Lógica relacionada con los servicios ///////
  ///////////////////////////////////////////////////

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

  ///////////////////////////////////////////////////
  /// Lógica relacionada con los Métodos de Pago ////
  ///////////////////////////////////////////////////

  cargarMetodosPago() {
    this.paymentMethodService.getAll().subscribe({
      next: (data) => this.paymentMethods = data,
      error: (err) => console.error('Error al obtener métodos de pago:', err)
    });
  }

  crearMetodoPago() {
    if (!this.newPaymentMethod.Method || !this.newPaymentMethod.Installments ||
      !this.newPaymentMethod.FirstPaymentDays || !this.newPaymentMethod.DaysBetweenPayments) {
      alert("Todos los campos son obligatorios");
      return;
    }

    this.paymentMethodService.create(this.newPaymentMethod).subscribe({
      next: (nuevo) => {
        this.paymentMethods.push(nuevo);
        this.newPaymentMethod = { Method: '', Installments: 0, FirstPaymentDays: 0, DaysBetweenPayments: 0}
      },
      error: (err) => {
        console.error('Error al crear método de pago:', err);
        alert("Error al crear método de pago");
      }
    });
  }
}
