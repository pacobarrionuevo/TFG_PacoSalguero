package com.example.tfg_movil.views

import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
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
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.collectAsState
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Brush
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import com.example.tfg_movil.R
import com.example.tfg_movil.model.customer.CustomerDTO
import com.example.tfg_movil.viewmodel.ViewModelCustomer
@Composable
fun CustomerScreen(viewModel: ViewModelCustomer) {
    // Estados y datos
    val customers by viewModel.customers.collectAsState()
    val error by viewModel.error.collectAsState()
    val context = LocalContext.current

    // Estados para campos del formulario
    var cif by remember { mutableStateOf("") }
    var name by remember { mutableStateOf("") }
    var adress by remember { mutableStateOf("") }
    var postalCode by remember { mutableStateOf("") }
    var placeOfResidence by remember { mutableStateOf("") }
    var phoneNumber by remember { mutableStateOf("") }
    var email by remember { mutableStateOf("") }
    var adminEmail by remember { mutableStateOf("") }
    var paymentMethodId by remember { mutableStateOf("") }

    LaunchedEffect(Unit) {
        viewModel.loadCustomers()
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
                                text = stringResource(id = R.string.crearPaciente),
                                style = MaterialTheme.typography.headlineMedium,
                                color = MaterialTheme.colorScheme.primary,
                                textAlign = TextAlign.Center
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            // Campos del formulario organizados en filas
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                TextField(
                                    value = cif,
                                    onValueChange = { cif = it },
                                    label = { Text(stringResource(id = R.string.cif)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp),
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number)
                                )
                                TextField(
                                    value = name,
                                    onValueChange = { name = it },
                                    label = { Text(stringResource(id = R.string.nombre)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp)
                                )
                            }

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = adress,
                                onValueChange = { adress = it },
                                label = { Text(stringResource(id = R.string.direccion)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                TextField(
                                    value = postalCode,
                                    onValueChange = { postalCode = it },
                                    label = { Text(stringResource(id = R.string.codigoPostal)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp),
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number)
                                )
                                TextField(
                                    value = placeOfResidence,
                                    onValueChange = { placeOfResidence = it },
                                    label = { Text(stringResource(id = R.string.lugarResidencia)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp)
                                )
                            }

                            Spacer(modifier = Modifier.height(12.dp))

                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.spacedBy(8.dp)
                            ) {
                                TextField(
                                    value = phoneNumber,
                                    onValueChange = { phoneNumber = it },
                                    label = { Text(stringResource(id = R.string.telefono)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp),
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Phone)
                                )
                                TextField(
                                    value = paymentMethodId,
                                    onValueChange = { paymentMethodId = it },
                                    label = { Text(stringResource(id = R.string.idMetodoPago)) },
                                    modifier = Modifier.weight(1f),
                                    shape = RoundedCornerShape(12.dp),
                                    keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number)
                                )
                            }

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = email,
                                onValueChange = { email = it },
                                label = { Text(stringResource(id = R.string.Email)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp),
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = adminEmail,
                                onValueChange = { adminEmail = it },
                                label = { Text(stringResource(id = R.string.emailAdministrativo)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp),
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email)
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            // Botón de creación y validación del cliente
                            Button(
                                onClick = {
                                    if (
                                        cif.toIntOrNull() != null &&
                                        postalCode.toIntOrNull() != null &&
                                        phoneNumber.toIntOrNull() != null &&
                                        paymentMethodId.toIntOrNull() != null
                                    ) {
                                        val dto = CustomerDTO(
                                            cif = cif.toInt(),
                                            name = name,
                                            adress = adress,
                                            postalCode = postalCode.toInt(),
                                            placeOfResidence = placeOfResidence,
                                            phoneNumber = phoneNumber.toInt(),
                                            email = email,
                                            adminEmail = adminEmail,
                                            paymentMethodId = paymentMethodId.toInt()
                                        )
                                        viewModel.createCustomer(dto)
                                        cif = ""; name = ""; adress = ""; postalCode = ""
                                        placeOfResidence = ""; phoneNumber = ""; email = ""; adminEmail = ""; paymentMethodId = ""
                                    } else {
                                        Toast.makeText(context, "Revisa los campos numéricos", Toast.LENGTH_SHORT).show()
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
                                    text = stringResource(id = R.string.crearPaciente),
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
                        text = stringResource(id = R.string.listaPacientes),
                        style = MaterialTheme.typography.headlineSmall,
                        color = MaterialTheme.colorScheme.onSurface,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(16.dp))
                }

                // Lista de clientes
                items(customers) { customer ->
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
                            // Customer info in a more organized layout
                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween
                            ) {
                                Column(modifier = Modifier.weight(1f)) {
                                    Text(
                                        text = customer.name,
                                        style = MaterialTheme.typography.titleMedium,
                                        color = MaterialTheme.colorScheme.onSurface
                                    )
                                    Text(
                                        text = "CIF: ${customer.cif}",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }
                                Column(horizontalAlignment = Alignment.End) {
                                    Text(
                                        text = "${stringResource(R.string.telefono)}: ${customer.phoneNumber}",
                                        style = MaterialTheme.typography.bodySmall,
                                        color = MaterialTheme.colorScheme.onSurfaceVariant
                                    )
                                }
                            }

                            Spacer(modifier = Modifier.height(8.dp))

                            Text(
                                text = "${stringResource(R.string.Email)}: ${customer.email}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )

                            Text(
                                text = "${stringResource(R.string.direccion)}: ${customer.adress}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )

                            Row(
                                modifier = Modifier.fillMaxWidth(),
                                horizontalArrangement = Arrangement.SpaceBetween
                            ) {
                                Text(
                                    text = "${stringResource(R.string.codigoPostal)}: ${customer.postalCode}",
                                    style = MaterialTheme.typography.bodySmall,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                                Text(
                                    text = "${stringResource(R.string.lugarResidencia)}: ${customer.placeOfResidence}",
                                    style = MaterialTheme.typography.bodySmall,
                                    color = MaterialTheme.colorScheme.onSurfaceVariant
                                )
                            }

                            Text(
                                text = "${stringResource(R.string.emailAdministrativo)}: ${customer.adminEmail}",
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )

                            Text(
                                text = "${stringResource(R.string.idMetodoPago)}: ${customer.paymentMethodId}",
                                style = MaterialTheme.typography.bodySmall,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            Row(
                                horizontalArrangement = Arrangement.End,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                IconButton(
                                    onClick = {
                                        viewModel.updateCustomer(
                                            customer.id,
                                            customer.copy(name = "Actualizado")
                                        )
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
                                        viewModel.deleteCustomer(customer.id)
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

