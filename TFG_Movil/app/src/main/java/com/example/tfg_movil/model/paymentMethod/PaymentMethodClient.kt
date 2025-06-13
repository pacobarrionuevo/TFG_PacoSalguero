package com.example.tfg_movil.model.paymentMethod

import retrofit2.Response
import retrofit2.http.*
// Interfaz para API de métodos de pago
interface PaymentMethodClient {
    @GET("api/PaymentMethod")
    suspend fun getAll(): List<PaymentMethod>

    @POST("api/PaymentMethod")
    suspend fun create(@Body method: PaymentMethod): Response<PaymentMethod>

    @PUT("api/PaymentMethod/{id}")
    suspend fun update(@Path("id") id: Int, @Body method: PaymentMethod): Response<PaymentMethod>

    @DELETE("api/PaymentMethod/{id}")
    suspend fun delete(
        @Path("id") id: Int,
        @Header("Authorization") token: String
    ): Response<Unit>
}