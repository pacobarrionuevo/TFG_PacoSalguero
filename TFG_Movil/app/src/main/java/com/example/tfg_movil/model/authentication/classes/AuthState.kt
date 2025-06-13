package com.example.tfg_movil.model.authentication.classes

// Estados posibles del flujo de autenticación
sealed class AuthState {
    object Idle : AuthState()
    object Loading : AuthState()
    data class Success(val message: String) : AuthState()
    data class Error(val message: String) : AuthState()
    data class Authenticated(val accessToken: String, val email: String, val userId: Int) : AuthState()
    object SignedOut : AuthState()
}