package com.example.tfg_movil.model.authentication.classes

data class AuthRequest(
    val email: String,
    val password: String,
    val role: String = "user"
)