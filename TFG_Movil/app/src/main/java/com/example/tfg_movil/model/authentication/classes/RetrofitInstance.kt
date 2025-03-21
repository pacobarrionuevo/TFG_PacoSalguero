package com.example.tfg_movil.model.authentication.classes

import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitInstance {

    // https://localhost:7077 es nuestro servidor
    private const val BASE_URL = "http://10.0.2.2:8080"

    private val retrofit by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
    }

    val authClient: AuthClient by lazy {
        retrofit.create(AuthClient::class.java)
    }
}