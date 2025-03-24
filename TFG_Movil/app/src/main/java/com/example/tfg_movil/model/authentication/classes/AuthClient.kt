package com.example.tfg_movil.model.authentication.classes

import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.POST

interface AuthClient {
    //
    @POST("/api/User/login")
    fun login(@Body authRequest: AuthRequest): Call<LoginResponse>

    @POST("/api/User/register")
    fun signUp(@Body authRequest: AuthRequest): Call<SignUpResponse>

}