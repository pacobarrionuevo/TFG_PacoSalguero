import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AgendaService } from '../../services/agenda.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Service } from '../../models/service';
import { ServicesService } from '../../services/services.service';
import { EntradaAgenda } from '../../models/entrada-agenda';
import { Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';

@Component({
  selector: 'app-crearentrada',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './crearentrada.component.html',
  styleUrl: './crearentrada.component.css'
})
export class CrearentradaComponent {
  entradaForm: FormGroup;
  servicios: Service[] = [];
  customers: Customer[] = [];
  constructor(
    private servicesService: ServicesService,
    private customerService: CustomerService,
    private fb: FormBuilder,
    private agendaService: AgendaService,
    public router: Router
  ) {
    this.cargarServicios();
    this.cargarCustomers();
    this.entradaForm = this.fb.group({
      cliente: ['', Validators.required],
      centroTrabajo: ['', Validators.required],
      servicioId: [null, Validators.required],
      precio: [0, [Validators.required, Validators.min(0)]],
      paciente: [null],
      observaciones: [''],
      fechaHora: ['', Validators.required],
    });
  }
  cargarServicios(): void {
    this.servicesService.getAll().subscribe({
      next: (servicios) => this.servicios = servicios,
      error: (err) => console.error('Error al cargar servicios:', err)
    });
  }

  cargarCustomers(): void {
    this.customerService.getAll().subscribe({
      next: (customers) => this.customers = customers,
      error: (err) => console.error('Error al cargar clientes:', err)
    });
  }
  ngOnInit(): void {
    this.servicesService.getAll().subscribe(servicios => {
      this.cargarServicios();
    });
  }
  onSubmit(): void {
  if (this.entradaForm.valid) {
    const formValues = this.entradaForm.value;

    console.log('Fecha del formulario:', formValues.fechaHora);
    const fechaHora = new Date(formValues.fechaHora);
    const fechaHoraUTC = new Date(fechaHora.getTime() - (fechaHora.getTimezoneOffset() * 60000));
    
    console.log('Fecha procesada:', fechaHoraUTC);

    const nuevaEntrada: EntradaAgenda = {
      cliente: formValues.cliente,
      centroTrabajo: formValues.centroTrabajo,
      serviceId: +formValues.servicioId, 
      precio: formValues.precio,
      paciente: formValues.paciente,
      observaciones: formValues.observaciones,
      fechaHora: fechaHoraUTC 
    };

    // Debug: Mostrar el objeto completo antes de enviar
    console.log('Entrada a crear:', nuevaEntrada);

    this.agendaService.crearEntrada(nuevaEntrada).subscribe({
      next: () => {
        this.router.navigate(['/agenda']);
        alert("Entrada creada correctamente.");
      },
      error: (err) => {
        console.error('Error al crear entrada desde Angular:', err);
        let errorMessage = 'Error al crear la entrada.';
        if (err.error) {
          if (typeof err.error === 'string') {
            errorMessage = err.error;
          } else if (err.error.errors) {
            const validationErrors = err.error.errors;
            errorMessage += '\nDetalles de validación:';
            for (const key in validationErrors) {
              if (validationErrors.hasOwnProperty(key)) {
                errorMessage += `\n- ${key}: ${validationErrors[key].join(', ')}`;
              }
            }
          }
        } else if (err.status) {
          errorMessage += `\nCódigo de estado HTTP: ${err.status} - ${err.statusText}`;
        }
        alert(errorMessage);
      }
    });
  } else {
    this.entradaForm.markAllAsTouched();
    alert('Por favor, completa todos los campos requeridos y asegúrate de que los valores son válidos.');
  }
}
todayISOString(): string {
  const now = new Date();
  // Convertir a formato YYYY-MM-DDTHH:MM
  return now.toISOString().slice(0, 16);
}
}