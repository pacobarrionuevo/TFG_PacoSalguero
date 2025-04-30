
export interface EntradaAgenda {
    id: number;
    fecha: Date;
    hora: string; // O TimeSpan
    cliente: string;
    centroTrabajo: string;
    servicio: string;
    precio: number;
    paciente?: string;
    observaciones?: string;
  }