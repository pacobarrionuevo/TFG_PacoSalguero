package com.example.tfg_movil.model.customer

import com.example.tfg_movil.model.authentication.classes.RetrofitInstance

// Repositorio para manejar operaciones de clientes
class CustomerRepository {
    private val api = RetrofitInstance.customerClient

    suspend fun fetchCustomers() = runCatching { api.getAllCustomers() }

    suspend fun createCustomer(dto: CustomerDTO) = runCatching { api.createCustomer(dto) }

    suspend fun updateCustomer(id: Int, dto: CustomerDTO) = runCatching { api.updateCustomer(id, dto) }

    suspend fun deleteCustomer(id: Int) = runCatching { api.deleteCustomer(id) }
}

