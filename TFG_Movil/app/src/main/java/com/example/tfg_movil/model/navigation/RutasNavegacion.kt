package com.example.tfg_movil.model.navigation

// Rutas de navegación principales
sealed class RutasNavegacion(val route: String) {
    object Main : RutasNavegacion("Main")
    object Login : RutasNavegacion("Login")
    object Register: RutasNavegacion("Register")
    object Servicios : RutasNavegacion("Servicios")
    object Customer : RutasNavegacion("Customer")
    object PaymentMethod : RutasNavegacion("PaymentMethod")
    object Agenda : RutasNavegacion("Agenda")
    object Calendar : RutasNavegacion("Calendar")

}