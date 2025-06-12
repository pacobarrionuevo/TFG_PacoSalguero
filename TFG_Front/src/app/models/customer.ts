import { PaymentMethod } from "./payment-method";

// Define la estructura de un cliente en el frontend.
export interface Customer {
    id?: number;
    cif: number | null;
    name: string;
    adress: string;
    postalCode: number | null;
    placeOfResidence: string;
    phoneNumber: number | null;
    email: string;
    adminEmail: string;
    paymentMethodId?: number;
    paymentMethod?: PaymentMethod;
    // Propiedad opcional para controlar el estado de edición en la UI.
    editing?: boolean;
}