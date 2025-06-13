package com.example.tfg_movil.model.paymentMethod


import com.google.gson.annotations.SerializedName
// Modelo de metodo de pago
data class PaymentMethod(
    @SerializedName("id")
    val id: Int,

    @SerializedName("method")
    val method: String,

    @SerializedName("installments")
    val installments: Int,

    @SerializedName("firstPaymentDays")
    val firstPaymentDays: Int,

    @SerializedName("daysBetweenPayments")
    val daysBetweenPayments: Int
)
