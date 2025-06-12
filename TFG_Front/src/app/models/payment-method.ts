// Define la estructura de un método de pago.
export interface PaymentMethod {
    id?: number;
    method: String;
    installments: number;
    firstPaymentDays: number;
    daysBetweenPayments: number;
    // Propiedad opcional para controlar el estado de edición en la UI.
    editing?: boolean;
}