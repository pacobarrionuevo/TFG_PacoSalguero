package com.example.tfg_movil.model.authentication.classes

import okhttp3.MultipartBody
import okhttp3.RequestBody
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.Multipart
import retrofit2.http.POST
import retrofit2.http.Part

// Interfaz para las llamadas de autenticación al servidor
interface AuthClient {
    @POST("/api/User/login")
    suspend fun login(@Body authRequest: AuthRequest): LoginResponse

    @Multipart
    @POST("/api/User/register")
    suspend fun signUp(
        @Part("UserNickname") nickname: RequestBody,
        @Part("UserEmail") email: RequestBody,
        @Part("UserPassword") password: RequestBody,
        @Part("UserConfirmPassword") confirmPassword: RequestBody,
        @Part UserProfilePhoto: MultipartBody.Part
    ): SignUpResponse
}