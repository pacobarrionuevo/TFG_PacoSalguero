
export interface EntradaAgenda {
    id: number;
    fecha: Date;
    hora: Date | string; 
    cliente: string;
    centroTrabajo: string;
    servicio: string;
    precio: number;
    paciente?: string;
    observaciones?: string;
  }