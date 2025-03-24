package com.example.tfg_movil.model.authentication

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.map

val Context.dataStoreAuth by preferencesDataStore(name = "auth_preferences")

class DataStoreManager {

    companion object {
        private val ACCESS_TOKEN_KEY = stringPreferencesKey("access_token")
        private val REFRESH_TOKEN_KEY = stringPreferencesKey("refresh_token")
        private val EMAIL_KEY = stringPreferencesKey("email")
        private val ID_KEY = stringPreferencesKey("id")

        suspend fun saveCredentials(
            context: Context, accessToken: String, refreshToken: String, email: String, id: String
        ) {
            context.dataStoreAuth.edit { preferences ->
                preferences[ACCESS_TOKEN_KEY] = accessToken
                preferences[REFRESH_TOKEN_KEY] = refreshToken
                preferences[EMAIL_KEY] = email
                preferences[ID_KEY] = id
            }
        }

        fun getAccessToken(context: Context) = context.dataStoreAuth.data.map { preferences ->
            preferences[ACCESS_TOKEN_KEY]
        }

        suspend fun getAccessTokenSync(context: Context): String? {
            return context.dataStoreAuth.data.map { preferences ->
                preferences[ACCESS_TOKEN_KEY]
            }.firstOrNull()
        }

        fun getRefreshToken(context: Context) = context.dataStoreAuth.data.map { preferences ->
            preferences[REFRESH_TOKEN_KEY]
        }

        fun getEmail(context: Context) = context.dataStoreAuth.data.map { preferences ->
            preferences[EMAIL_KEY]
        }

        suspend fun clearCredentials(context: Context) {
            context.dataStoreAuth.edit { preferences -> preferences.clear() }
        }

        fun getUserId(context: Context) = context.dataStoreAuth.data.map { preferences ->
            preferences[ID_KEY]
        }
    }
}