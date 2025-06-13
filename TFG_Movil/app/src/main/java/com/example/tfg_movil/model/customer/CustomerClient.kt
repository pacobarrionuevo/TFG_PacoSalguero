package com.example.tfg_movil.model.customer

import retrofit2.http.*
import retrofit2.Response

// Interfaz Retrofit para operaciones CRUD de clientes
interface CustomerClient {

    @GET("/api/Customer")
    suspend fun getAllCustomers(): List<Customer>

    @POST("/api/Customer")
    suspend fun createCustomer(@Body customer: CustomerDTO): Response<Customer>

    @PUT("/api/Customer/{id}")
    suspend fun updateCustomer(@Path("id") id: Int, @Body customer: CustomerDTO): Response<Customer>

    @DELETE("/api/Customer/{id}")
    suspend fun deleteCustomer(@Path("id") id: Int): Response<Unit>
}

