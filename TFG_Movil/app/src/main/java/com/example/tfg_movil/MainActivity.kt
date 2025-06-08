package com.example.tfg_movil

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.compose.rememberNavController
import com.example.tfg_movil.model.authentication.classes.AuthRepository
import com.example.tfg_movil.model.navigation.AppNavigation
import com.example.tfg_movil.model.navigation.NavigationDrawer
import com.example.tfg_movil.ui.theme.TFG_MovilTheme
import com.example.tfg_movil.viewmodel.ViewModelAuth

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            val navController = rememberNavController()
            val context = LocalContext.current

            val authViewModel: ViewModelAuth = viewModel { ViewModelAuth(AuthRepository(), context) }

            LaunchedEffect(Unit) {
            }

            TFG_MovilTheme {
                NavigationDrawer(navController) {
                    AppNavigation(navController, authViewModel.authState.collectAsState().value, authViewModel)
                }
            }
        }
    }
}