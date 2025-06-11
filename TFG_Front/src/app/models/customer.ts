import { PaymentMethod } from "./payment-method";

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
    editing?: boolean;
}