package com.example.tfg_movil.model.agenda

class AgendaRepository(private val client: AgendaClient) {

    // Declaración de los métodos del AgendaClient que se van a usar en el ViewModel.

    suspend fun getAll() = runCatching { client.getEntradas() }

    suspend fun getEntradasPorMes(year: Int, month: Int) = runCatching {
        client.getEntradasPorMes(year, month)
    }

    suspend fun create(entrada: EntradaAgenda) = runCatching {
        val response = client.create(entrada)
        if (!response.isSuccessful) {
            throw Exception("Error ${response.code()}: ${response.errorBody()?.string()}")
        }
        response.body() ?: throw Exception("Respuesta vacía")
    }
}