package com.example.tfg_movil.views

import android.util.Log
import android.widget.Toast
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Lock
import androidx.compose.material.icons.filled.Warning
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.input.PasswordVisualTransformation
import androidx.compose.ui.text.input.VisualTransformation
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import com.example.tfg_movil.R
import com.example.tfg_movil.model.authentication.classes.AuthState
import com.example.tfg_movil.model.navigation.RutasNavegacion
import com.example.tfg_movil.viewmodel.ViewModelAuth

@Composable
fun Login(viewModelAuth: ViewModelAuth, navController: NavController) {
    var email by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var user_id by remember { mutableStateOf("") }
    var passwordVisible by remember { mutableStateOf(false) }
    val authState by viewModelAuth.authState.collectAsState()
    val context = LocalContext.current

    LaunchedEffect(authState) {
        when (authState) {
            is AuthState.Authenticated -> navController.navigate(RutasNavegacion.Main.route)
            is AuthState.Error -> {
                Toast.makeText(context, (authState as AuthState.Error).message, Toast.LENGTH_SHORT).show()
                viewModelAuth.resetAuthState()
                Log.d("AuthState", "Reiniciado")
            }
            else -> Unit
        }
    }

    Column(
        modifier = Modifier.fillMaxSize(),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text(
            text = stringResource(id = R.string.Login),
            style = MaterialTheme.typography.displayMedium
        )

        Spacer(modifier = Modifier.height(16.dp))

        OutlinedTextField(
            value = email,
            onValueChange = { email = it },
            label = { Text(text = stringResource(id = R.string.Email)) }
        )

        Spacer(modifier = Modifier.height(8.dp))

        OutlinedTextField(
            value = password,
            onValueChange = { password = it },
            label = { Text(text = stringResource(id = R.string.Password)) },
            visualTransformation = if (passwordVisible) VisualTransformation.None else PasswordVisualTransformation(),
            trailingIcon = {
                val image = if (passwordVisible) Icons.Filled.Lock else Icons.Filled.Warning
                val description = if (passwordVisible) {
                    stringResource(id = R.string.ShowPassword)
                } else {
                    stringResource(id = R.string.HidePassword)
                }

                IconButton(onClick = { passwordVisible = !passwordVisible }) {
                    Icon(imageVector = image, contentDescription = description)
                }
            }
        )

        Spacer(modifier = Modifier.height(16.dp))

        Button(
            onClick = { viewModelAuth.login(email, password ) },
            enabled = authState != AuthState.Loading,
            colors = ButtonDefaults.buttonColors(
                containerColor = MaterialTheme.colorScheme.onPrimaryContainer,
                contentColor = MaterialTheme.colorScheme.primary
            )
        ) {
            Text(text = stringResource(id = R.string.Login))
        }

        Spacer(modifier = Modifier.height(8.dp))

        TextButton(onClick = { navController.navigate(RutasNavegacion.Register.route) }) {
            Text(text = stringResource(id = R.string.MessageToLogin))
        }
    }
}