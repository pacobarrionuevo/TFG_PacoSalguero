package com.example.tfg_movil.model.navigation

import androidx.compose.ui.graphics.vector.ImageVector
// Modelo para items del menú de navegación
data class NavigationItems(
    val title: String,
    val selectedIcon: ImageVector,
    val unselectedIcon: ImageVector,
    val route: String,
    val badgeCount: Int? = null
)