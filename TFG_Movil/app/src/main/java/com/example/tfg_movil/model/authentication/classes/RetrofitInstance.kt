package com.example.tfg_movil.model.authentication.classes

import android.annotation.SuppressLint
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.*

object RetrofitInstance {
    // Para emulador Android (10.0.2.2 = localhost del host)
    private const val BASE_URL = "https://10.0.2.2:7077"  // Asegúrate que coincide con tu puerto HTTPS del servidor C#

    // Cliente HTTP personalizado para desarrollo (acepta certificados autofirmados)
    private val unsafeOkHttpClient = createUnsafeOkHttpClient()

    private val retrofit by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(unsafeOkHttpClient)  // Usa nuestro cliente inseguro (solo desarrollo)
            .addConverterFactory(GsonConverterFactory.create())
            .build()
    }

    val authClient: AuthClient by lazy {
        retrofit.create(AuthClient::class.java)
    }

    // Crea un OkHttpClient que ignore errores SSL (SOLO PARA DESARROLLO)
    @SuppressLint("CustomX509TrustManager")
    private fun createUnsafeOkHttpClient(): OkHttpClient {
        val trustAllCerts = arrayOf<TrustManager>(object : X509TrustManager {
            override fun checkClientTrusted(chain: Array<X509Certificate>, authType: String) {}
            override fun checkServerTrusted(chain: Array<X509Certificate>, authType: String) {}
            override fun getAcceptedIssuers(): Array<X509Certificate> = arrayOf()
        })

        val sslContext = SSLContext.getInstance("SSL").apply {
            init(null, trustAllCerts, SecureRandom())
        }

        return OkHttpClient.Builder()
            .sslSocketFactory(sslContext.socketFactory, trustAllCerts[0] as X509TrustManager)
            .hostnameVerifier { _, _ -> true }  // Ignora verificación de hostname
            .build()
    }
}