package com.example.tfg_movil.model.authentication.classes

import com.example.tfg_movil.model.agenda.AgendaClient
import com.example.tfg_movil.model.customer.CustomerClient
import com.example.tfg_movil.model.services.ServiceClient
import com.example.tfg_movil.model.paymentMethod.PaymentMethodClient
import com.google.gson.GsonBuilder
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import java.security.SecureRandom
import java.security.cert.X509Certificate
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

// Configuración de Retrofit para conexiones HTTPS
object RetrofitInstance {

    // Para emulador Android (10.0.2.2 = localhost del host)
    // La de development es "https://10.0.2.2:7077/"
    private const val BASE_URL = "https://10.0.2.2:7077/"
    private val trustAllCerts = arrayOf<TrustManager>(object : X509TrustManager {
        override fun checkClientTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
        override fun checkServerTrusted(chain: Array<out X509Certificate>?, authType: String?) {}
        override fun getAcceptedIssuers() = arrayOf<X509Certificate>()
    })
    // Configuración para aceptar todos los certificados
    private val unsafeOkHttpClient = OkHttpClient.Builder()
        .sslSocketFactory(
            SSLContext.getInstance("SSL").apply {
                init(null, trustAllCerts, SecureRandom())
            }.socketFactory,
            trustAllCerts[0] as X509TrustManager
        )
        .hostnameVerifier { _, _ -> true }
        .build()

    private val gson = GsonBuilder()
        .setDateFormat("yyyy-MM-dd'T'HH:mm:ss")
        .create()

    private val retrofit by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL)
            .client(unsafeOkHttpClient)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .build()
    }

    val authClient: AuthClient by lazy {
        retrofit.create(AuthClient::class.java)
    }
    val serviceClient: ServiceClient by lazy {
        retrofit.create(ServiceClient::class.java)
    }
    val customerClient: CustomerClient by lazy {
        retrofit.create(CustomerClient::class.java)
    }
    val paymentMethodClient: PaymentMethodClient by lazy {
        retrofit.create(PaymentMethodClient::class.java)
    }

    val agendaClient: AgendaClient by lazy {
        retrofit.create(AgendaClient::class.java)
    }
}