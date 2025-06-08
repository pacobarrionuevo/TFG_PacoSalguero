import { Component, OnInit } from '@angular/core';
import { Service } from '../../models/service';
import { ServicesService } from '../../services/services.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentMethod } from '../../models/payment-method';
import { PaymentMethodService } from '../../services/payment-method.service';
import { Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-ficheros',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './ficheros.component.html',
  styleUrls: ['./ficheros.component.css']
})
export class FicherosComponent implements OnInit {
  servicios: Service[] = [];
  paymentMethods: PaymentMethod[] = [];
  customers: Customer[] = [];

  nuevoServicio: Service = {
    nombre: '',
    abreviatura: '',
    color: ''
  };

  newPaymentMethod: PaymentMethod = {
    method: '',
    installments: 0,
    firstPaymentDays: 0,
    daysBetweenPayments: 0
  };

  newCustomer: Customer = {
    cif: 0,
    name: '',
    adress: '',
    postalCode: 0,
    placeOfResidence: '',
    phoneNumber: 0,
    email: '',
    adminEmail: '',
    paymentMethod: this.newPaymentMethod,
  }

  constructor(private servicioService: ServicesService, private paymentMethodService: PaymentMethodService,
    private customerService: CustomerService) {}

  ngOnInit(): void {
    this.cargarServicios();
    this.cargarMetodosPago();
    this.cargarClientes();
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
  editarServicio(servicio: Service): void {
    servicio.editing = true;
  }
  guardarServicio(servicio: Service): void {
    this.servicioService.update(servicio).subscribe({
      next: () => {
        servicio.editing = false;
        this.cargarServicios();
      },
      error: (err) => console.error('Error al actualizar servicio:', err)
    });
  }

  eliminarServicio(id: number): void {
    if (confirm('¿Estás seguro de eliminar este servicio?')) {
      this.servicioService.delete(id).subscribe({
        next: () => this.cargarServicios(),
        error: (err) => console.error('Error al eliminar servicio:', err)
      });
    }
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
    if (!this.newPaymentMethod.method ||
        this.newPaymentMethod.installments < 1 ||
        this.newPaymentMethod.firstPaymentDays < 0 ||
        this.newPaymentMethod.daysBetweenPayments < 0) {
      alert("Todos los campos son obligatorios y deben tener valores válidos");
      return;
    }


    this.paymentMethodService.create(this.newPaymentMethod).subscribe({
      next: (nuevo) => {
        console.log('Método de pago creado:', nuevo);
        this.paymentMethods.push(nuevo);
        this.newPaymentMethod = { method: '', installments: 0, firstPaymentDays: 0, daysBetweenPayments: 0}
      },
      error: (err) => {
        console.error('Error al crear método de pago:', err);
        alert("Error al crear método de pago");
      }
    });
  }

  ///////////////////////////////////////////////////
  /////// Lógica relacionada con los clientes ///////
  ///////////////////////////////////////////////////

  cargarClientes() {
    this.customerService.getAll().subscribe({
      next: (data) => this.customers = data,
      error: (err) => console.error('Error al obtener métodos de pago:', err)
    });
  }

  crearClientes() {
  // Validación básica
  if (
    !this.newCustomer.name ||
    !this.newCustomer.email ||
    !this.newCustomer.paymentMethodId ||
    this.newCustomer.cif <= 0 ||
    this.newCustomer.phoneNumber <= 0
  ) {
    alert("Faltan campos obligatorios o hay valores inválidos.");
    return;
  }

  // Crear objeto con solo los campos que necesita el backend
  const clienteDTO = {
    cif: this.newCustomer.cif,
    name: this.newCustomer.name,
    adress: this.newCustomer.adress,
    postalCode: this.newCustomer.postalCode,
    placeOfResidence: this.newCustomer.placeOfResidence,
    phoneNumber: this.newCustomer.phoneNumber,
    email: this.newCustomer.email,
    adminEmail: this.newCustomer.adminEmail, 
    paymentMethodId: this.newCustomer.paymentMethodId 
};

  this.customerService.create(clienteDTO).subscribe({
    next: (clienteCreado) => {
      console.log('Cliente creado:', clienteCreado);
      this.customers.push(clienteCreado);
      alert("Cliente creado correctamente");

      // Reiniciar formulario
      this.newCustomer = {
        cif: 0,
        name: '',
        adress: '',
        postalCode: 0,
        placeOfResidence: '',
        phoneNumber: 0,
        email: '',
        adminEmail: '',
        paymentMethodId: undefined,
        paymentMethod: undefined
      };
    },
    error: (err) => {
      console.error('Error al crear cliente:', err);
      alert("Error al crear cliente");
    }
  });
}


}
