package com.example.tfg_movil.model.authentication.classes

import com.google.gson.annotations.SerializedName

// Respuesta del login
data class LoginResponse(
    @SerializedName("stringToken") val accessToken: String,
    @SerializedName("userId") val userId: Int
)