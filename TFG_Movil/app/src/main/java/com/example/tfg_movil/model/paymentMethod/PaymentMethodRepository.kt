package com.example.tfg_movil.model.paymentMethod

// Interfaz para API de métodos de pago
class PaymentMethodRepository(
    private val client: PaymentMethodClient
) {
    suspend fun fetchAll() = runCatching {
        client.getAll()
    }

    suspend fun create(method: PaymentMethod) = runCatching {
        val response = client.create(method)
        if (!response.isSuccessful) {
            throw Exception("HTTP ${response.code()} - ${response.errorBody()?.string()}")
        }
        response.body() ?: throw Exception("Respuesta vacía")
    }

    suspend fun update(id: Int, method: PaymentMethod) = runCatching {
        val response = client.update(id, method)
        if (!response.isSuccessful) {
            throw Exception("HTTP ${response.code()} - ${response.errorBody()?.string()}")
        }
        response.body() ?: throw Exception("Respuesta vacía")
    }

    suspend fun delete(id: Int, token: String) = runCatching {
        val response = client.delete(id, "Bearer $token")
        when {
            response.code() == 404 -> throw Exception("El método de pago no existe")
            !response.isSuccessful -> throw Exception("Error ${response.code()}: ${response.errorBody()?.string()}")
            else -> Unit
        }
    }
}