package com.example.tfg_movil.model.services

import com.google.gson.annotations.SerializedName

data class Service(
    @SerializedName("id")
    val id: Int,

    @SerializedName("nombre")
    val nombre: String,

    @SerializedName("abreviatura")
    val abreviatura: String,

    @SerializedName("color")
    val color: String
) {
    constructor() : this(0, "", "", "")
}