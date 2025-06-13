package com.example.tfg_movil.model.agenda

import com.example.tfg_movil.model.services.Service
import com.google.gson.annotations.SerializedName
import java.util.*

data class EntradaAgenda(
    val id: Int = 0,
    @SerializedName("fechaHora")
    val fechaHora: Date,
    val cliente: String,
    val centroTrabajo: String,
    val serviceId: Int,
    val paciente: String? = null,
    val precio: Double,
    val observaciones: String? = null,
    val service: Service? = null
)