package com.example.tfg_movil.viewmodel

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.example.tfg_movil.model.authentication.DataStoreManager
import com.example.tfg_movil.model.services.Service
import com.example.tfg_movil.model.services.ServiceRepository
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

// ViewModel para gestión de servicios
class ViewModelService(
    application: Application,
    private val repository: ServiceRepository
) : AndroidViewModel(application) {

    private val _services = MutableStateFlow<List<Service>>(emptyList())
    val services: StateFlow<List<Service>> = _services

    private val context get() = getApplication<Application>().applicationContext

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Servicio en edicion
    private val _editingService = MutableStateFlow<Service?>(null)
    val editingService: StateFlow<Service?> = _editingService

    fun loadServices() {
        viewModelScope.launch {
            repository.fetchServices()
                .onSuccess { services ->
                    _services.value = services
                    _error.value = null
                }
                .onFailure { e ->
                    _error.value = "Error cargando servicios: ${e.message}"
                }
        }
    }

    // Operaciones CRUD
    fun createService(service: Service) {
        viewModelScope.launch {
            repository.createService(service)
                .onSuccess {
                    _error.value = "¡Servicio creado!"
                    loadServices()
                }
                .onFailure {
                    _error.value = "Error creando servicio: ${it.message}"
                }
        }
    }

    fun updateService(service: Service) {
        viewModelScope.launch {
            repository.updateService(service.id, service)
                .onSuccess { updatedService ->
                    _services.value = _services.value.map {
                        if (it.id == service.id) updatedService else it
                    }
                    _editingService.value = null
                }
                .onFailure { e ->
                    _error.value = "Error actualizando: ${e.message}"
                }
        }
    }

    fun deleteService(id: Int) {
        println("Intentando eliminar servicio con ID: $id")
        viewModelScope.launch {
            val token = DataStoreManager.getAccessTokenSync(context) ?: ""
            println("Token recuperado: '$token'")
            repository.deleteService(id, token)
                .onSuccess {
                    println("Servicio eliminado con éxito")
                    _services.value = _services.value.filter { it.id != id }
                }
                .onFailure { e ->
                    println("Fallo al eliminar: ${e.message}")
                    _error.value = when {
                        e.message?.contains("404") == true -> "El servicio ya no existe"
                        else -> "Error al eliminar: ${e.message}"
                    }
                }
        }
    }

    fun startEditing(service: Service) {
        _editingService.value = service
    }

    fun cancelEditing() {
        _editingService.value = null
    }
}