package com.example.tfg_movil.viewmodel

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.example.tfg_movil.model.agenda.AgendaRepository
import com.example.tfg_movil.model.agenda.EntradaAgenda
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch

// ViewModel para la gestión de la Agenda
class ViewModelAgenda(
    application: Application,
    private val repository: AgendaRepository
) : AndroidViewModel(application) {

    // Flujo mutable para almacenar las entradas de agenda
    private val _entradas = MutableStateFlow<List<EntradaAgenda>>(emptyList())
    val entradas: StateFlow<List<EntradaAgenda>> = _entradas

    private val _error = MutableStateFlow<String?>(null)
    val error: StateFlow<String?> = _error

    // Carga todas las entradas de la agenda
    fun cargarTodas() {
        viewModelScope.launch {
            repository.getAll()
                .onSuccess { _entradas.value = it }
                .onFailure { _error.value = it.message }
        }
    }

    // Crea una nueva entrada en la agenda
    fun crearEntrada(entrada: EntradaAgenda) {
        viewModelScope.launch {
            repository.create(entrada)
                .onSuccess {
                    cargarTodas()
                    _error.value = null
                }
                .onFailure { _error.value = it.message }
        }
    }

    // Carga entradas para un mes específico
    fun cargarMes(year: Int, month: Int) {
        viewModelScope.launch {
            repository.getEntradasPorMes(year, month)
                .onSuccess { _entradas.value = it }
                .onFailure { _error.value = it.message }
        }
    }
}