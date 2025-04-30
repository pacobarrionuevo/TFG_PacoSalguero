import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AgendaService } from '../../services/agenda.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-crearentrada',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './crearentrada.component.html',
  styleUrl: './crearentrada.component.css'
})
export class CrearentradaComponent {
  entradaForm: FormGroup;
  constructor(
    private fb: FormBuilder,
    private agendaService: AgendaService,
    public router: Router
  ) {
    this.entradaForm = this.fb.group({
      cliente: ['', Validators.required],
      centroTrabajo: ['', Validators.required],
      servicio: ['', Validators.required],
      precio: [0, [Validators.required, Validators.min(0)]],
      paciente: [''],
      observaciones: [''],
      hora: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.entradaForm.valid) {
      const nuevaEntrada = {
        ...this.entradaForm.value,
        fecha: new Date().toISOString().split('T')[0] // Pone el Formato YYYY-MM-DD
      };

      this.agendaService.crearEntrada(nuevaEntrada).subscribe({
        next: () => {
          this.router.navigate(['/agenda']); 
        },
        error: (err) => {
          console.error('Error al crear entrada:', err);
          alert('Error al crear la entrada');
        }
      });
    }
  }
}