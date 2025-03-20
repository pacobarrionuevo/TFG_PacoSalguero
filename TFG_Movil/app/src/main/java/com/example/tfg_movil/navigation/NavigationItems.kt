package com.example.tfg_movil.navigation

import androidx.compose.ui.graphics.vector.ImageVector

data class NavigationItems(
    val title: String,
    val selectedIcon: ImageVector,
    val unselectedIcon: ImageVector,
    val route: String,
    val badgeCount: Int? = null
)