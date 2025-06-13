package com.example.tfg_movil.viewmodel

import android.content.ContentResolver
import android.content.Context
import android.util.Log
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import coil3.Uri
import com.example.tfg_movil.model.authentication.DataStoreManager
import com.example.tfg_movil.model.authentication.classes.AuthRepository
import com.example.tfg_movil.model.authentication.classes.AuthState
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

// ViewModel para manejar autenticación
class ViewModelAuth(
    private val authRepository: AuthRepository,
    context: Context
) : ViewModel() {

    private val _authState = MutableStateFlow<AuthState>(AuthState.Idle)
    val authState: StateFlow<AuthState> = _authState

    private val appContext = context.applicationContext

    // Maneja login con email/contraseña
    fun login(emailOrNickname: String, password: String) {
        _authState.value = AuthState.Loading

        viewModelScope.launch(Dispatchers.IO) {
            // Lógica de autenticación
            try {
                val result = authRepository.login(emailOrNickname, password)

                withContext(Dispatchers.Main) {
                    if (result.isSuccess) {
                        val loginResponse = result.getOrNull()
                        if (loginResponse != null) {
                            DataStoreManager.saveCredentials(
                                appContext,
                                loginResponse.accessToken,
                                emailOrNickname,
                                loginResponse.userId
                            )
                            _authState.value = AuthState.Authenticated(
                                accessToken = loginResponse.accessToken,
                                email = emailOrNickname,
                                userId = loginResponse.userId
                            )
                        } else {
                            _authState.value = AuthState.Error("Credenciales incorrectas.")
                        }
                    } else {
                        val errorMessage = result.exceptionOrNull()?.message ?: "Login fallido."
                        _authState.value = AuthState.Error(errorMessage)
                    }
                }
            } catch (e: Exception) {
                Log.e("Auth", "Excepción durante el login: ${e.message}", e)
                withContext(Dispatchers.Main) {
                    _authState.value = AuthState.Error("Error inesperado durante el login.")
                }
            }
        }
    }

    // Registra nuevo usuario
    fun signUp(nickname: String, email: String, password: String, confirmPassword: String,
                profilePhotoUri: android.net.Uri, contentResolver: ContentResolver) {
        _authState.value = AuthState.Loading

        viewModelScope.launch(Dispatchers.IO) {
            try {
                val result = authRepository.signUp(nickname, email, password, confirmPassword, profilePhotoUri, contentResolver)

                withContext(Dispatchers.Main) {
                    if (result.isSuccess) {
                        val response = result.getOrNull()
                        if (response != null) {
                            DataStoreManager.saveCredentials(
                                appContext,
                                response.accessToken,
                                email,
                                userId = -1
                            )
                            _authState.value = AuthState.Authenticated(
                                accessToken = response.accessToken,
                                email = email,
                                userId = -1
                            )
                        } else {
                            _authState.value = AuthState.Error("Error en el servidor.")
                        }
                    } else {
                        val errorMessage = result.exceptionOrNull()?.message ?: "Registro fallido."
                        _authState.value = AuthState.Error(errorMessage)
                    }
                }
            } catch (e: Exception) {
                Log.e("Auth", "Excepción durante el registro: ${e.message}", e)
                withContext(Dispatchers.Main) {
                    _authState.value = AuthState.Error("Error inesperado durante el registro.")
                }
            }
        }
    }

    fun signOut() {
        viewModelScope.launch {
            DataStoreManager.clearCredentials(appContext)
            _authState.value = AuthState.SignedOut
        }
    }

    fun resetAuthState() {
        _authState.value = AuthState.Idle
    }

    // Carga credenciales guardadas, como el token y otros datos varios
    fun loadCredentials() {
        viewModelScope.launch {
            val accessToken = DataStoreManager.getAccessToken(appContext).firstOrNull()
            val email = DataStoreManager.getEmail(appContext).firstOrNull()
            val userId = DataStoreManager.getUserId(appContext)

            if (accessToken != null && email != null && userId != null) {
                _authState.value = AuthState.Authenticated(
                    accessToken = accessToken,
                    email = email,
                    userId = userId
                )
            } else {
                _authState.value = AuthState.SignedOut
            }
        }
    }
}


