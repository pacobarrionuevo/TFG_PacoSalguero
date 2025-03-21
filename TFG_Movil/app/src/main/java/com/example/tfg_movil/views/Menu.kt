package com.example.tfg_movil.views

import androidx.compose.foundation.layout.Column
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.navigation.NavController
import com.example.tfg_movil.viewmodel.ViewModelAuth

@Composable
fun Menu(viewModelAuth: ViewModelAuth, navController: NavController) {
    LaunchedEffect(Unit) { }

    Column {
        Text("Menu works!");
    }

}