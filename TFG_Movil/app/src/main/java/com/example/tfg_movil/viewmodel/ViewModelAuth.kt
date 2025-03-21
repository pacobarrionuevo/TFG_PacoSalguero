package com.example.tfg_movil.viewmodel

import android.content.Context
import android.util.Log
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.tfg_movil.model.authentication.DataStoreManager
import com.example.tfg_movil.model.authentication.classes.AuthRepository
import com.example.tfg_movil.model.authentication.classes.AuthRequest
import com.example.tfg_movil.model.authentication.classes.AuthState
import com.example.tfg_movil.model.authentication.classes.SignUpResponse
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.first
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class ViewModelAuth(
    private val authRepository: AuthRepository,
    context: Context
) : ViewModel() {

    private val _authState = MutableStateFlow<AuthState>(AuthState.Idle)
    val authState: StateFlow<AuthState> = _authState

    private val _userDetails = MutableStateFlow<SignUpResponse?>(null)
    val userDetails: StateFlow<SignUpResponse?> = _userDetails

    private val appContext = context.applicationContext

    fun login(email: String, password: String, user_id: String) {
        _authState.value = AuthState.Loading

        viewModelScope.launch(Dispatchers.IO) {
            try {
                val result = authRepository.login(email, password)

                withContext(Dispatchers.Main) {
                    if (result.isSuccess) {
                        val loginResponse = result.getOrNull()
                        if (loginResponse != null) {

                            DataStoreManager.saveCredentials(
                                appContext,
                                loginResponse.accessToken,
                                loginResponse.refreshToken,
                                email,
                                user_id
                            )
                            _authState.value = AuthState.Authenticated(
                                loginResponse.accessToken,
                                loginResponse.refreshToken,
                                email
                            )

                        } else {

                            _authState.value = AuthState.Error("Error en el servidor.")
                        }
                    } else {
                        val errorMessage = result.exceptionOrNull()?.message ?: "Login fallido."
                        _authState.value = AuthState.Error(errorMessage)
                    }
                }
            } catch (e: Exception) {
                Log.e("Auth", "Excepción durante el login: ${e.message}", e)
                withContext(Dispatchers.Main) {
                    _authState.value = AuthState.Error("An unexpected error occurred.")
                }
            }
        }
    }



    fun signUp(email: String, password: String, user_id: String) {
        _authState.value = AuthState.Loading

        viewModelScope.launch(Dispatchers.IO) {
            val result = authRepository.signUp(email, password)

            withContext(Dispatchers.Main) {
                if (result.isSuccess) {
                    login(email, password, user_id )
                    _authState.value = AuthState.Success("User registered successfully.")
                } else {
                    val errorMessage = result.exceptionOrNull()?.message ?: "Registration failed."

                    _authState.value = AuthState.Error(errorMessage)
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



    fun loadCredentials() {
        viewModelScope.launch {
            val accessToken = DataStoreManager.getAccessToken(appContext).first()
            val refreshToken = DataStoreManager.getRefreshToken(appContext).first()
            val email = DataStoreManager.getEmail(appContext).first()
            if (accessToken != null && refreshToken != null && email != null) {
                _authState.value = AuthState.Authenticated(accessToken, refreshToken, email)
            } else {
                _authState.value = AuthState.Idle
            }
        }
    }
}