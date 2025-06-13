package com.example.tfg_movil.model.authentication.classes

import android.content.ContentResolver
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.MultipartBody
import okhttp3.RequestBody.Companion.toRequestBody

// Repositorio que maneja la logica de autenticacion
class AuthRepository {

    private val authClient = RetrofitInstance.authClient

    suspend fun login(emailOrNickname: String, password: String): Result<LoginResponse> {
        return try {
            // Usamos los nombres exactos que espera el backend
            println(" login request: ${AuthRequest(emailOrNickname, password)}")
            val response = authClient.login(AuthRequest(
                userEmailOrNickname = emailOrNickname,
                password = password
            ))
            println("Login response: $response")

            Result.success(response)
        } catch (e: Exception) {
            println("Login error: ${e.message}")
            e.printStackTrace()
            Result.failure(e)
        }
    }

    suspend fun signUp(
        nickname: String,
        email: String,
        password: String,
        confirmPassword: String,
        profilePhotoUri: android.net.Uri,
        contentResolver: ContentResolver
    ): Result<SignUpResponse> {
        return try {
            // Convierte la imagen a MultipartBody.Part para la peticion
            val photoStream = contentResolver.openInputStream(profilePhotoUri)!!
            val photoBytes = photoStream.readBytes()
            photoStream.close()

            val requestBodyPhoto = photoBytes.toRequestBody("image/*".toMediaTypeOrNull())
            val multipartPhoto = MultipartBody.Part.createFormData(
                "UserProfilePhoto",
                "profile.jpg",
                requestBodyPhoto
            )

            val response = authClient.signUp(
                nickname.toRequestBody("text/plain".toMediaTypeOrNull()),
                email.toRequestBody("text/plain".toMediaTypeOrNull()),
                password.toRequestBody("text/plain".toMediaTypeOrNull()),
                confirmPassword.toRequestBody("text/plain".toMediaTypeOrNull()),
                multipartPhoto
            )
            Result.success(response)
        } catch (e: Exception) {
            Result.failure(e)
        }
    }
}
