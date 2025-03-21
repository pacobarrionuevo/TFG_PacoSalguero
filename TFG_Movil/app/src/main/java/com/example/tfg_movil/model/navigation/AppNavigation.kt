package com.example.tfg_movil.model.navigation

import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.width
import androidx.compose.material3.NavigationDrawerItem
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.getValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.lifecycle.viewmodel.compose.viewModel
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
import kotlinx.coroutines.launch
import androidx.compose.foundation.layout.height
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.DrawerValue
import androidx.compose.material3.Icon
import androidx.compose.material3.ModalDrawerSheet
import androidx.compose.runtime.remember
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.unit.dp
import com.example.tfg_movil.R


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
                AplicacionPrincipal(navController, authViewModel, tareasViewModel)
            }
            composable(RutasNavegacion.InicioSesion.route) {
                Login(authViewModel, navController)
            }
            composable(RutasNavegacion.Registro.route) {
                Register(authViewModel, navController)
            }
            composable(RutasNavegacion.SobreNosotros.route) {
                Menu(navController)
            }
            }
        }
    }
}