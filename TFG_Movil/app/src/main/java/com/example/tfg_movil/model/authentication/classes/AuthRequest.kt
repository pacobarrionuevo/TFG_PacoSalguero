package com.example.tfg_movil.model.authentication.classes

import com.google.gson.annotations.SerializedName

// Modelo para la petición de login
data class AuthRequest(
    @SerializedName("UserEmailOrNickname") val userEmailOrNickname: String,
    @SerializedName("UserPassword") val password: String
)