// Define la estructura de un servicio que se va a facturar.
export interface ServiceFacturado {
  centro: string;
  cliente: string;
  fecha: Date;
  paciente?: string;
  servicio: string;
  observaciones?: string;
}