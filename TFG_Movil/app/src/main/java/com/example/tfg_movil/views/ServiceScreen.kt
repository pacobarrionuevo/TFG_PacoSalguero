package com.example.tfg_movil.views

import android.app.Application
import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Edit
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.IconButton
import androidx.compose.material3.MaterialTheme
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import androidx.lifecycle.viewmodel.compose.viewModel
import com.example.tfg_movil.model.authentication.classes.RetrofitInstance
import com.example.tfg_movil.model.services.Service
import com.example.tfg_movil.viewmodel.ViewModelFactory
import com.example.tfg_movil.viewmodel.ViewModelService
import com.example.tfg_movil.model.services.ServiceRepository
import androidx.compose.runtime.getValue
import androidx.compose.runtime.setValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import com.example.tfg_movil.R

@Composable
fun ServiceScreen() {
    val context = LocalContext.current
    val application = context.applicationContext as Application
    val viewModel: ViewModelService = viewModel(
        factory = ViewModelFactory(application, ServiceRepository(RetrofitInstance.serviceClient))
    )
    val services by viewModel.services.collectAsState()
    val editingService by viewModel.editingService.collectAsState()
    val error by viewModel.error.collectAsState()

    var nombre by remember(editingService) {
        mutableStateOf(editingService?.nombre ?: "")
    }
    var abreviatura by remember(editingService) {
        mutableStateOf(editingService?.abreviatura ?: "")
    }
    var color by remember(editingService) {
        mutableStateOf(editingService?.color ?: "")
    }

    fun esColorHexValido(valor: String): Boolean {
        return Regex("^#[0-9a-fA-F]{6}$").matches(valor)
    }
    val colorValido = esColorHexValido(color)

    LaunchedEffect(Unit) {
        viewModel.loadServices()
    }

    LaunchedEffect(editingService) {
        nombre = editingService?.nombre ?: ""
        abreviatura = editingService?.abreviatura ?: ""
        color = editingService?.color ?: ""
    }

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
        Spacer(modifier = Modifier.height(75.dp))
        Box(
            modifier = Modifier
                .fillMaxSize()
                .background(brush = backgroundGradient)
                .padding(padding)
        ) {
            LazyColumn(
                modifier = Modifier
                    .fillMaxSize()
                    .padding(16.dp),
                horizontalAlignment = Alignment.CenterHorizontally
            ) {
                item {
                    Spacer(modifier = Modifier.height(75.dp))
                    Card(
                        modifier = Modifier
                            .fillMaxWidth(0.95f)
                            .padding(vertical = 8.dp),
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
                                text = if (editingService != null) stringResource(id = R.string.editarServicio)
                                else stringResource(id = R.string.crearServicio),
                                style = MaterialTheme.typography.headlineMedium,
                                color = MaterialTheme.colorScheme.primary,
                                textAlign = TextAlign.Center
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            TextField(
                                value = nombre,
                                onValueChange = { nombre = it },
                                label = { Text(stringResource(id = R.string.nombre)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = abreviatura,
                                onValueChange = { abreviatura = it },
                                label = { Text(stringResource(id = R.string.abreviatura)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = color,
                                onValueChange = { color = it },
                                label = { Text(stringResource(id = R.string.color)) },
                                isError = !colorValido && color.isNotEmpty(),
                                supportingText = {
                                    if (!colorValido && color.isNotEmpty()) {
                                        Text(
                                            text = stringResource(id = R.string.colorRequirement),
                                            color = MaterialTheme.colorScheme.error
                                        )
                                    }
                                },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            Button(
                                onClick = {
                                    val nuevoServicio = Service(
                                        id = editingService?.id ?: 0,
                                        nombre = nombre,
                                        abreviatura = abreviatura,
                                        color = color
                                    )

                                    if (editingService != null) {
                                        viewModel.updateService(nuevoServicio)
                                    } else {
                                        viewModel.createService(nuevoServicio)
                                    }

                                    nombre = ""; abreviatura = ""; color = ""
                                },
                                modifier = Modifier
                                    .fillMaxWidth()
                                    .height(48.dp),
                                shape = RoundedCornerShape(12.dp),
                                colors = ButtonDefaults.buttonColors(
                                    containerColor = MaterialTheme.colorScheme.primary
                                )
                            ) {
                                Text(
                                    text = if (editingService != null) stringResource(id = R.string.GuardarCambios)
                                    else stringResource(id = R.string.crearServicio),
                                    style = MaterialTheme.typography.labelLarge
                                )
                            }

                            if (error != null) {
                                Spacer(modifier = Modifier.height(16.dp))
                                Text(
                                    text = "Error: $error",
                                    color = MaterialTheme.colorScheme.error,
                                    style = MaterialTheme.typography.bodyMedium
                                )
                            }
                        }
                    }
                }

                item {
                    Spacer(modifier = Modifier.height(24.dp))

                    Text(
                        text = stringResource(id = R.string.listaServicios),
                        style = MaterialTheme.typography.headlineSmall,
                        color = MaterialTheme.colorScheme.inverseSurface,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(16.dp))
                }

                items(services) { service ->
                    Card(
                        modifier = Modifier
                            .fillMaxWidth(0.95f)
                            .padding(vertical = 6.dp),
                        colors = CardDefaults.cardColors(
                            containerColor = MaterialTheme.colorScheme.surface
                        ),
                        elevation = CardDefaults.cardElevation(
                            defaultElevation = 4.dp
                        ),
                        shape = RoundedCornerShape(16.dp)
                    ) {
                        Column(
                            modifier = Modifier.padding(16.dp)
                        ) {
                            Text(
                                text = "${stringResource(R.string.nombre)}: ${service.nombre}",
                                style = MaterialTheme.typography.bodyLarge,
                                color = MaterialTheme.colorScheme.onSurface
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.abreviatura)}: ${service.abreviatura}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Row(
                                verticalAlignment = Alignment.CenterVertically
                            ) {
                                Text(
                                    text = "${stringResource(R.string.color)}: ${service.color}",
                                    style = MaterialTheme.typography.bodyMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                                Spacer(modifier = Modifier.width(8.dp))
                                if (esColorHexValido(service.color)) {
                                    Box(
                                        modifier = Modifier
                                            .size(16.dp)
                                            .background(
                                                color = Color(android.graphics.Color.parseColor(service.color)),
                                                shape = CircleShape
                                            )
                                            .border(
                                                1.dp,
                                                MaterialTheme.colorScheme.outline,
                                                CircleShape
                                            )
                                    )
                                }
                            }

                            Spacer(modifier = Modifier.height(12.dp))

                            Row(
                                horizontalArrangement = Arrangement.End,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                IconButton(
                                    onClick = {
                                        viewModel.startEditing(service)
                                        println("edit clicked")
                                    },
                                    modifier = Modifier
                                        .background(
                                            MaterialTheme.colorScheme.primaryContainer,
                                            CircleShape
                                        )
                                ) {
                                    Icon(
                                        Icons.Default.Edit,
                                        contentDescription = "Editar",
                                        tint = MaterialTheme.colorScheme.onPrimaryContainer
                                    )
                                }
                                Spacer(modifier = Modifier.width(8.dp))
                                IconButton(
                                    onClick = {
                                        if (service.id != 0) {
                                            viewModel.deleteService(service.id)
                                        } else {
                                            Toast.makeText(context, "ID inválido", Toast.LENGTH_SHORT).show()
                                        }
                                        println("borrao clicked")
                                    },
                                    modifier = Modifier
                                        .background(
                                            MaterialTheme.colorScheme.errorContainer,
                                            CircleShape
                                        )
                                ) {
                                    Icon(
                                        Icons.Default.Delete,
                                        contentDescription = "Eliminar",
                                        tint = MaterialTheme.colorScheme.onErrorContainer
                                    )
                                }
                            }
                        }
                    }
                }

                item {
                    Spacer(modifier = Modifier.height(24.dp))
                }
            }
        }
    }
}
