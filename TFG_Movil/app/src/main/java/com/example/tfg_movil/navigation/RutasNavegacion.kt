package com.example.tfg_movil.navigation

sealed class RutasNavegacion(val route: String) {
    object Main : RutasNavegacion("Main")
    object InicioSesion :RutasNavegacion("InicioSesion")
    object Registro: RutasNavegacion("Registro")
    object Menu: RutasNavegacion("Menu")
}