package com.example.tfg_movil.model.customer

// Modelo de datos para lo que será un Paciente
data class Customer(
    val id: Int,
    val cif: Int,
    val name: String,
    val adress: String,
    val postalCode: Int,
    val placeOfResidence: String,
    val phoneNumber: Int,
    val email: String,
    val adminEmail: String,
    val paymentMethodId: Int
)

