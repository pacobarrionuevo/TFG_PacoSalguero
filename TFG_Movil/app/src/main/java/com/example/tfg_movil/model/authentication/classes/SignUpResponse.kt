package com.example.tfg_movil.model.authentication.classes

import com.google.gson.annotations.SerializedName

// Respuesta del registro
data class SignUpResponse(
    @SerializedName("stringToken")
    val accessToken: String
)
