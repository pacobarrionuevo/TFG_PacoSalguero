export interface PaymentMethod {
    id?: number;
    Method: String;
    Installments: number;
    FirstPaymentDays: number;
    DaysBetweenPayments: number;
}