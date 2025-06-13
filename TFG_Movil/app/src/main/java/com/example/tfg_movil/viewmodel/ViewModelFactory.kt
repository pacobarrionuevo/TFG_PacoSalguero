package com.example.tfg_movil.viewmodel

import android.app.Application
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.tfg_movil.model.services.ServiceRepository

// Factory personalizada para crear instancias de ViewModel con dependencias
class ViewModelFactory(

    // Application es necesario para ViewModels que necesitan contexto
    private val application: Application,
    private val repository: ServiceRepository
) : ViewModelProvider.Factory {

    // Metodo requerido para crear ViewModels
    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel> create(modelClass: Class<T>): T {

        // Crea y devuelve una instancia del ViewModelService con las dependencias necesarias pa que funcione bien
        return ViewModelService(
            application,
            repository
        ) as T
    }
}

