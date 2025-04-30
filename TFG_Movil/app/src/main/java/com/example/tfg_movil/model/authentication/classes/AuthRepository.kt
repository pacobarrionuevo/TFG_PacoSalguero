package com.example.tfg_movil.model.authentication.classes

class AuthRepository {

    private val authClient = RetrofitInstance.authClient

    // De momento, lo único que tenemos es el registro
    // Ya añadiremos las cosas del usuario que ha iniciado sesión

    suspend fun login(email: String, password: String): Result<LoginResponse> {
        return try {
            val response = authClient.login(AuthRequest(email, password)).execute()
            if (response.isSuccessful) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Login fallido: ${response.errorBody()?.string()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

    suspend fun signUp(email: String, password: String): Result<SignUpResponse> {
        return try {
            val response = authClient.signUp(AuthRequest(email, password, "USER")).execute()
            if (response.isSuccessful) {
                Result.success(response.body()!!)
            } else {
                Result.failure(Exception("Registro fallido: ${response.errorBody()?.string()}"))
            }
        } catch (e: Exception) {
            Result.failure(e)
        }
    }

}