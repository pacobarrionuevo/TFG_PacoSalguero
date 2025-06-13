package com.example.tfg_movil.viewmodel

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.example.tfg_movil.model.paymentMethod.PaymentMethod
import com.example.tfg_movil.model.paymentMethod.PaymentMethodRepository
import com.example.tfg_movil.model.authentication.DataStoreManager
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
// ViewModel para métodos de pago

class ViewModelPaymentMethod(
    application: Application,
    private val repository: PaymentMethodRepository
) : AndroidViewModel(application) {

    private val context get() = getApplication<Application>().applicationContext

    private val _methods = MutableStateFlow<List<PaymentMethod>>(emptyList())
    val methods: StateFlow<List<PaymentMethod>> = _methods

    // Edición de métodos
    private val _editing = MutableStateFlow<PaymentMethod?>(null)
    val editing: StateFlow<PaymentMethod?> = _editing

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    fun loadMethods() {
        viewModelScope.launch {
            repository.fetchAll()
                .onSuccess { _methods.value = it }
                .onFailure { _error.value = "Error cargando métodos: ${it.message}" }
        }
    }

    // Operaciones CRUD...
    fun create(method: PaymentMethod) {
        viewModelScope.launch {
            repository.create(method)
                .onSuccess {
                    loadMethods()
                }
                .onFailure { _error.value = "Error creando: ${it.message}" }
        }
    }

    fun update(method: PaymentMethod) {
        viewModelScope.launch {
            repository.update(method.id, method)
                .onSuccess {
                    _methods.value = _methods.value.map {
                        if (it.id == method.id) it.copy(
                            method = method.method,
                            installments = method.installments,
                            firstPaymentDays = method.firstPaymentDays,
                            daysBetweenPayments = method.daysBetweenPayments
                        ) else it
                    }
                    _editing.value = null
                }
                .onFailure { _error.value = "Error actualizando: ${it.message}" }
        }
    }

    fun delete(id: Int) {
        viewModelScope.launch {
            val token = DataStoreManager.getAccessTokenSync(context) ?: ""
            repository.delete(id, token)
                .onSuccess {
                    _methods.value = _methods.value.filter { it.id != id }
                }
                .onFailure {
                    _error.value = "Error al eliminar: ${it.message}"
                }
        }
    }

    fun startEditing(method: PaymentMethod) {
        _editing.value = method
    }

    fun cancelEditing() {
        _editing.value = null
    }
}