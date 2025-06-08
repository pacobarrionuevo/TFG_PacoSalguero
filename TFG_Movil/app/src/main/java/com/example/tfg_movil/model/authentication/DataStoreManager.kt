package com.example.tfg_movil.model.authentication

import android.content.Context
import androidx.datastore.preferences.core.edit
import androidx.datastore.preferences.core.intPreferencesKey
import androidx.datastore.preferences.core.stringPreferencesKey
import androidx.datastore.preferences.preferencesDataStore
import kotlinx.coroutines.flow.firstOrNull
import kotlinx.coroutines.flow.map

val Context.dataStoreAuth by preferencesDataStore(name = "auth_preferences")

class DataStoreManager {

    companion object {
        private val ACCESS_TOKEN_KEY = stringPreferencesKey("access_token")
        private val EMAIL_KEY = stringPreferencesKey("email")
        private val USER_ID_KEY  = intPreferencesKey("id")

        suspend fun saveCredentials(
            context: Context, accessToken: String, email: String, userId: Int
        ) {
            context.dataStoreAuth.edit { preferences ->
                preferences[ACCESS_TOKEN_KEY] = accessToken
                preferences[EMAIL_KEY] = email
                preferences[USER_ID_KEY] = userId
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

        fun getEmail(context: Context) = context.dataStoreAuth.data.map { preferences ->
            preferences[EMAIL_KEY]
        }

        suspend fun clearCredentials(context: Context) {
            context.dataStoreAuth.edit { preferences -> preferences.clear() }
        }

        suspend fun getUserId(context: Context): Int? {
            return context.dataStoreAuth.data.map { preferences ->
                preferences[USER_ID_KEY]
            }.firstOrNull()
        }
    }
}