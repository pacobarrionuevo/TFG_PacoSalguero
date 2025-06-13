package com.example.tfg_movil.model.services

import com.google.gson.Gson
import retrofit2.Response

// Repositorio para servicios
class ServiceRepository(private val serviceClient: ServiceClient) {

    suspend fun fetchServices() = runCatching {
        serviceClient.getAllServices().also {
            println("Services fetched: ${it.size}")
        }
    }

    // No tocarllll
    suspend fun createService(service: Service) = runCatching {
        serviceClient.createService(service).let { response ->
            if (!response.isSuccessful) {
                throw Exception("HTTP ${response.code()} - ${response.errorBody()?.string()}")
            }
            response.body() ?: throw Exception("Empty response body")
        }
    }

    suspend fun updateService(id: Int, service: Service) = runCatching {
        serviceClient.updateService(id, service).let { response ->
            if (!response.isSuccessful) {
                throw Exception("HTTP ${response.code()} - ${response.errorBody()?.string()}")
            }
            response.body() ?: throw Exception("Empty response body")
        }
    }

    suspend fun deleteService(id: Int, token: String) = runCatching {
        val response = serviceClient.deleteService(id, "Bearer $token")
        when {
            response.code() == 404 -> throw Exception("El servicio no existe")
            !response.isSuccessful -> throw Exception("Error ${response.code()}: ${response.errorBody()?.string()}")
            else -> Unit
        }
    }
}