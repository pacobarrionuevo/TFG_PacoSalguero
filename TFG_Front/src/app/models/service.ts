// Define la estructura de un servicio.
export interface Service {
    id?: number;
    nombre: string;
    abreviatura: string;
    color: string;
    // Propiedad opcional para controlar el estado de edición en la UI.
    editing?: boolean;
  }