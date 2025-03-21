package com.example.tfg_movil.model.authentication.classes

data class LoginResponse(
    val accessToken: String,
    val refreshToken: String
)