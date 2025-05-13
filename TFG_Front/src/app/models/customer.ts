import { PaymentMethod } from "./payment-method";

export interface Customer {
    id?: number;
    CIF: number;
    name: string;
    adress: string;
    postalCode: number;
    placeOfResidence: string;
    phoneNumber: number;
    email: string;
    adminEmail: string;
    paymentMethodId?: number;
    paymentMethod?: PaymentMethod;
}