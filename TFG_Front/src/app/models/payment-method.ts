export interface PaymentMethod {
    id?: number;
    method: String;
    installments: number;
    firstPaymentDays: number;
    daysBetweenPayments: number;
    editing?: boolean;
}