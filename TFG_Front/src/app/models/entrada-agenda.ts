import { Service } from "./service";

export interface EntradaAgenda {
    id?: number;
    fecha: Date;
    hora: Date | string;
    cliente: string;
    centroTrabajo: string;
    serviceId: number;  
    paciente?: string;
    precio: number;
    observaciones?: string;
    servicio?: Service;     
}