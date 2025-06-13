package com.example.tfg_movil.model.services

import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.GET
import retrofit2.http.Header
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

// Interfaz para API de servicios
interface ServiceClient {
    @GET("api/Service/get_services")
    suspend fun getAllServices(): List<Service>

    @POST("api/Service/post_services")
    suspend fun createService(@Body service: Service): Response<Unit>

    @PUT("api/Service/{id}")
    suspend fun updateService(@Path("id") id: Int, @Body service: Service): Response<Service>

    @DELETE("api/Service/{id}")
    suspend fun deleteService(
        @Path("id") id: Int,
        @Header("Authorization") token: String
    ): Response<Unit>
}