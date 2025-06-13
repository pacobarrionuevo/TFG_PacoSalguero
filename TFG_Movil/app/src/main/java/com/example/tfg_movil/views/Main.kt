package com.example.tfg_movil.views

import androidx.compose.foundation.BorderStroke
import androidx.compose.foundation.Image
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.defaultMinSize
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.requiredSize
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ExitToApp
import androidx.compose.material.icons.filled.AccountCircle
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.CircularProgressIndicator
import androidx.compose.material3.Icon
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.layout.ContentScale
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.navigation.NavHostController
import com.example.tfg_movil.R
import com.example.tfg_movil.model.navigation.RutasNavegacion
import com.example.tfg_movil.model.authentication.classes.AuthState
import com.example.tfg_movil.model.authentication.DataStoreManager
import com.example.tfg_movil.viewmodel.ViewModelAuth

@Composable
fun Main(
    navController: NavHostController,
    authViewModel: ViewModelAuth
) {
    val authState by authViewModel.authState.collectAsState()

    if (authState is AuthState.Idle) {
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(MaterialTheme.colorScheme.surface),
            contentAlignment = Alignment.Center
        ) {
            CircularProgressIndicator(
                color = MaterialTheme.colorScheme.primary
            )
        }
    } else {
        MainContent(navController, authViewModel, authState)
    }
}

@Composable
fun MainContent(
    navController: NavHostController,
    authViewModel: ViewModelAuth,
    authState: AuthState
) {
    val context = LocalContext.current
    val isAuthenticated = authState is AuthState.Authenticated

    val backgroundGradient = Brush.verticalGradient(
        listOf(
            Color(0xFF9BB5D6),
            Color(0xFF6B9BD8),
            Color(0xFF4A7FB8)
        )
    )

    Scaffold(
        containerColor = MaterialTheme.colorScheme.surface
    ) { padding ->
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(brush = backgroundGradient)
                .padding(padding),
            contentAlignment = Alignment.Center
        ) {
            Card(
                modifier = Modifier
                    .padding(16.dp)
                    .fillMaxWidth(0.9f),
                colors = CardDefaults.cardColors(
                    containerColor = MaterialTheme.colorScheme.surface
                ),
                elevation = CardDefaults.cardElevation(
                    defaultElevation = 8.dp
                ),
                shape = RoundedCornerShape(20.dp)
            ) {
                Column(
                    modifier = Modifier
                        .padding(24.dp)
                        .fillMaxWidth(),
                    horizontalAlignment = Alignment.CenterHorizontally
                ) {
                    Text(
                        text = "¡Bienvenidos a Sanitarios App!",
                        style = MaterialTheme.typography.headlineMedium,
                        color = MaterialTheme.colorScheme.primary,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(24.dp))

                    Card(
                        modifier = Modifier
                            .fillMaxWidth()
                            .height(200.dp),
                        shape = RoundedCornerShape(16.dp),
                        elevation = CardDefaults.cardElevation(
                            defaultElevation = 4.dp
                        )
                    ) {
                        Image(
                            painter = painterResource(id = R.drawable.enfermeras),
                            contentDescription = "Foto enfermeras",
                            modifier = Modifier.fillMaxSize(),
                            contentScale = ContentScale.Crop
                        )
                    }

                    Spacer(modifier = Modifier.height(24.dp))

                    UserActionButton(navController, authViewModel)
                }
            }
        }
    }
}

@Composable
fun NavigationButton(text: String, onClick: () -> Unit) {
    Button(
        onClick = onClick,
        modifier = Modifier
            .fillMaxWidth()
            .padding(vertical = 4.dp),
        colors = ButtonDefaults.buttonColors(
            containerColor = MaterialTheme.colorScheme.primary,
            contentColor = MaterialTheme.colorScheme.onPrimary
        ),
        shape = RoundedCornerShape(12.dp)
    ) {
        Text(
            text = text,
            style = MaterialTheme.typography.labelLarge
        )
    }
}

@Composable
fun UserActionButton(navController: NavHostController, authViewModel: ViewModelAuth) {
    val authState by authViewModel.authState.collectAsState()

    Button(
        onClick = {
            if (authState is AuthState.Authenticated) {
                authViewModel.signOut()
            } else {
                navController.navigate(RutasNavegacion.Login.route)
            }
        },
        modifier = Modifier
            .fillMaxWidth()
            .height(64.dp),
        colors = ButtonDefaults.buttonColors(
            containerColor = MaterialTheme.colorScheme.primaryContainer,
            contentColor = MaterialTheme.colorScheme.onPrimaryContainer
        ),
        shape = RoundedCornerShape(32.dp), // Más redondeado como en la imagen
        elevation = ButtonDefaults.buttonElevation(
            defaultElevation = 4.dp
        )
    ) {
        Row(
            verticalAlignment = Alignment.CenterVertically,
            horizontalArrangement = Arrangement.Center
        ) {
            Icon(
                imageVector = if (authState is AuthState.Authenticated)
                    Icons.AutoMirrored.Filled.ExitToApp
                else
                    Icons.Default.AccountCircle,
                contentDescription = if (authState is AuthState.Authenticated)
                    stringResource(R.string.Logout)
                else
                    stringResource(R.string.Login),
                modifier = Modifier.requiredSize(32.dp)
            )
            Spacer(modifier = Modifier.width(12.dp))
            Text(
                text = if (authState is AuthState.Authenticated)
                    stringResource(R.string.Logout)
                else
                    stringResource(R.string.Login),
                style = MaterialTheme.typography.labelLarge
            )
        }
    }
}