package com.example.tfg_movil.model.agenda

import retrofit2.http.*
import retrofit2.Response

interface AgendaClient {
    // Para llamar a los endpoint del EntradaAgendaController

    // Recibe EntradaAgenda
    @GET("api/Agenda")
    suspend fun getEntradas(): List<EntradaAgenda>

    // Crea EntradaAgenda
    @POST("api/Agenda")
    suspend fun create(@Body entrada: EntradaAgenda): Response<EntradaAgenda>

    // Recibe las entradas según el mes
    @GET("api/Agenda/mes/{year}/{month}")
    suspend fun getEntradasPorMes(@Path("year") year: Int, @Path("month") month: Int): List<EntradaAgenda>
}