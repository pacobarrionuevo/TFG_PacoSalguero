package com.example.tfg_movil.model.authentication.classes

sealed class AuthState {
    object Idle : AuthState()
    object Loading : AuthState()
    data class Success(val message: String) : AuthState()
    data class Error(val message: String) : AuthState()
    data class Authenticated(val accessToken: String, val refreshToken: String, val email: String) : AuthState()
    object SignedOut : AuthState()
}