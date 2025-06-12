import { Service } from "./service";

// Define la estructura de una entrada de la agenda.
export interface EntradaAgenda {
    id?: number;
    fechaHora: Date;
    cliente: string;
    centroTrabajo: string;
    serviceId: number;  
    paciente?: string;
    precio: number;
    observaciones?: string;
    // Propiedad opcional para anidar la información completa del servicio.
    servicio?: Service;
    // Propiedad opcional para controlar si la entrada está seleccionada en la UI.
    seleccionado?: boolean;     
}