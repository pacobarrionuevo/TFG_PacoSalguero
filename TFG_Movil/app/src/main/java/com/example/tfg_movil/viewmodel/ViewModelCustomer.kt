package com.example.tfg_movil.viewmodel

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.tfg_movil.model.customer.Customer
import com.example.tfg_movil.model.customer.CustomerDTO
import com.example.tfg_movil.model.customer.CustomerRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
// ViewModel para gestión de clientes
class ViewModelCustomer(private val repository: CustomerRepository) : ViewModel() {

    private val _customers = MutableStateFlow<List<Customer>>(emptyList())
    val customers: StateFlow<List<Customer>> = _customers

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Carga todos los clientes
    fun loadCustomers() {
        viewModelScope.launch {
            repository.fetchCustomers()
                .onSuccess { _customers.value = it }
                .onFailure { _error.value = it.message }
        }
    }

    // CRUD de clientes
    fun createCustomer(dto: CustomerDTO) {
        viewModelScope.launch {
            repository.createCustomer(dto)
                .onSuccess { loadCustomers() }
                .onFailure { _error.value = it.message }
        }
    }

    fun updateCustomer(id: Int, customer: Customer) {
        viewModelScope.launch {
            val dto = CustomerDTO(
                id = customer.id,
                cif = customer.cif,
                name = customer.name,
                adress = customer.adress,
                postalCode = customer.postalCode,
                placeOfResidence = customer.placeOfResidence,
                phoneNumber = customer.phoneNumber,
                email = customer.email,
                adminEmail = customer.adminEmail,
                paymentMethodId = customer.paymentMethodId
            )
            repository.updateCustomer(id, dto)
                .onSuccess { loadCustomers() }
                .onFailure { _error.value = it.message }
        }
    }

    fun deleteCustomer(id: Int) {
        viewModelScope.launch {
            repository.deleteCustomer(id)
                .onSuccess { loadCustomers() }
                .onFailure { _error.value = it.message }
        }
    }
}
