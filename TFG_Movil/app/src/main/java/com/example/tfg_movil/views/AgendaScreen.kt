package com.example.tfg_movil.views

import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.tfg_movil.R
import com.example.tfg_movil.model.agenda.EntradaAgenda
import com.example.tfg_movil.viewmodel.ViewModelAgenda
import java.text.SimpleDateFormat
import java.util.*

@Composable
fun AgendaScreen(viewModel: ViewModelAgenda) {
    val entradas by viewModel.entradas.collectAsState()
    val error by viewModel.error.collectAsState()

    // Estados para los campos del formulario
    var cliente by remember { mutableStateOf("") }
    var centroTrabajo by remember { mutableStateOf("") }
    var serviceId by remember { mutableStateOf("") }
    var paciente by remember { mutableStateOf("") }
    var precio by remember { mutableStateOf("") }
    var observaciones by remember { mutableStateOf("") }
    var fechaHora by remember { mutableStateOf("") }

    var ordenDescendente by remember { mutableStateOf(true) }

    val context = LocalContext.current

    // Cargar entradas al iniciar
    LaunchedEffect(Unit) {
        viewModel.cargarTodas()
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
                    // Formulario para nueva entrada
                    Spacer(Modifier.height(100.dp))

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
                                text = stringResource(id = R.string.crearNuevaEntrada),
                                style = MaterialTheme.typography.headlineMedium,
                                color = MaterialTheme.colorScheme.primary,
                                textAlign = TextAlign.Center
                            )

                            Spacer(Modifier.height(24.dp))

                            TextField(
                                value = cliente,
                                onValueChange = { cliente = it },
                                label = { Text(stringResource(id = R.string.cliente)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = centroTrabajo,
                                onValueChange = { centroTrabajo = it },
                                label = { Text(stringResource(id = R.string.centroTrabajo)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = serviceId,
                                onValueChange = { serviceId = it },
                                label = { Text(stringResource(id = R.string.idServicio)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = paciente,
                                onValueChange = { paciente = it },
                                label = { Text(stringResource(id = R.string.paciente)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = precio,
                                onValueChange = { precio = it },
                                label = { Text(stringResource(id = R.string.precio)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = observaciones,
                                onValueChange = { observaciones = it },
                                label = { Text(stringResource(id = R.string.observaciones)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(Modifier.height(12.dp))

                            TextField(
                                value = fechaHora,
                                onValueChange = { fechaHora = it },
                                label = { Text(stringResource(id = R.string.fechaHora)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            Button(
                                onClick = {
                                    try {
                                        val parsedFecha = SimpleDateFormat("yyyy-MM-dd'T'HH:mm", Locale.getDefault()).parse(fechaHora)
                                        // 2025-06-15T10:30
                                        if (
                                            parsedFecha != null &&
                                            serviceId.toIntOrNull() != null &&
                                            precio.toDoubleOrNull() != null
                                        ) {
                                            val entrada = EntradaAgenda(
                                                fechaHora = parsedFecha,
                                                cliente = cliente,
                                                centroTrabajo = centroTrabajo,
                                                serviceId = serviceId.toInt(),
                                                paciente = paciente.takeIf { it.isNotBlank() },
                                                precio = precio.toDouble(),
                                                observaciones = observaciones.takeIf { it.isNotBlank() }
                                            )
                                            viewModel.crearEntrada(entrada)

                                            cliente = ""
                                            centroTrabajo = ""
                                            serviceId = ""
                                            paciente = ""
                                            precio = ""
                                            observaciones = ""
                                            fechaHora = ""

                                            Toast.makeText(context, "Entrada creada", Toast.LENGTH_SHORT).show()
                                        } else {
                                            Toast.makeText(context, "Campos numéricos o fecha inválidos", Toast.LENGTH_SHORT).show()
                                        }
                                    } catch (e: Exception) {
                                        Toast.makeText(context, "Error: ${e.message}", Toast.LENGTH_SHORT).show()
                                    }
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
                                    text = stringResource(id = R.string.crearNuevaEntrada),
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

                    // Listado de entradas
                    Text(
                        text = stringResource(id = R.string.listaEntradas),
                        style = MaterialTheme.typography.headlineSmall,
                        color = MaterialTheme.colorScheme.inverseSurface,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(8.dp))

                    // Filtro de fecha
                    Row(
                        modifier = Modifier
                            .fillMaxWidth(0.95f),
                        horizontalArrangement = Arrangement.SpaceBetween,
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Text(
                            text = "Ordenar por fecha:",
                            style = MaterialTheme.typography.bodyMedium,
                            color = MaterialTheme.colorScheme.inverseSurface
                        )

                        Row(verticalAlignment = Alignment.CenterVertically) {
                            RadioButton(
                                selected = ordenDescendente,
                                onClick = { ordenDescendente = true }
                            )
                            Text("Más recientes", style = MaterialTheme.typography.bodySmall)

                            Spacer(modifier = Modifier.width(8.dp))

                            RadioButton(
                                selected = !ordenDescendente,
                                onClick = { ordenDescendente = false }
                            )
                            Text("Más antiguas", style = MaterialTheme.typography.bodySmall)
                        }
                    }

                    Spacer(modifier = Modifier.height(16.dp))

                }
                val entradasOrdenadas = if (ordenDescendente) {
                    entradas.sortedByDescending { it.fechaHora }
                } else {
                    entradas.sortedBy { it.fechaHora }
                }
                // Items de la lista de entradas
                items(entradasOrdenadas) { entrada ->
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
                        Column(modifier = Modifier.padding(16.dp)) {
                            Text(
                                text = "${stringResource(R.string.fechaHora)}: ${entrada.fechaHora}",
                                style = MaterialTheme.typography.bodyLarge,
                                color = MaterialTheme.colorScheme.onSurface
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.cliente)}: ${entrada.cliente}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.centroTrabajo)}: ${entrada.centroTrabajo}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.servicio)}: ${entrada.service?.nombre ?: entrada.serviceId}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.precio)}: €${entrada.precio}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            entrada.paciente?.let {
                                Spacer(modifier = Modifier.height(4.dp))
                                Text(
                                    text = "${stringResource(R.string.paciente)}: $it",
                                    style = MaterialTheme.typography.bodyMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }
                            entrada.observaciones?.let {
                                Spacer(modifier = Modifier.height(4.dp))
                                Text(
                                    text = "${stringResource(R.string.observaciones)}: $it",
                                    style = MaterialTheme.typography.bodyMedium,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
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
