export interface ServiceFacturado {
  centro: string;
  cliente: string;
  fecha: Date;
  paciente?: string;
  servicio: string;
  observaciones?: string;
}