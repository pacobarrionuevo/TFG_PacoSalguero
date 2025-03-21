package com.example.tfg_movil.model.navigation

import androidx.compose.foundation.layout.Spacer
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Modifier
import androidx.navigation.NavHostController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.currentBackStackEntryAsState
import com.example.tfg_movil.views.Login
import com.example.tfg_movil.views.Register
import com.example.tfg_movil.model.authentication.classes.AuthState
import com.example.tfg_movil.viewmodel.ViewModelAuth
import androidx.compose.ui.platform.LocalContext
import androidx.compose.material3.ModalNavigationDrawer
import androidx.compose.material3.rememberDrawerState
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.foundation.layout.height
import androidx.compose.material3.DrawerValue
import androidx.compose.material3.ModalDrawerSheet
import androidx.compose.ui.unit.dp
import com.example.tfg_movil.views.Main
import com.example.tfg_movil.views.Menu


@Composable
fun AppNavigation(navController: NavHostController, authState: AuthState, authViewModel: ViewModelAuth) {
    val context = LocalContext.current
    val drawerState = rememberDrawerState(initialValue = DrawerValue.Closed)
    val scope = rememberCoroutineScope()
    val navBackStackEntry by navController.currentBackStackEntryAsState()
    val currentRoute = navBackStackEntry?.destination?.route
    val startDestination = when (authState) {
        is AuthState.Authenticated -> RutasNavegacion.Main.route
        else -> RutasNavegacion.Login.route
    }


    ModalNavigationDrawer(
        drawerState = drawerState,
        drawerContent = {
            ModalDrawerSheet {
                Spacer(Modifier.height(16.dp))
            }
        }
    ) {
        NavHost(navController = navController, startDestination = startDestination) {
            composable(RutasNavegacion.Main.route) {
                Main(navController, authViewModel)
            }
            composable(RutasNavegacion.Login.route) {
                Login(authViewModel, navController)
            }
            composable(RutasNavegacion.Register.route) {
                Register(authViewModel, navController)
            }
            composable(RutasNavegacion.Menu.route) {
                Menu(authViewModel, navController)
            }
        }
    }
}