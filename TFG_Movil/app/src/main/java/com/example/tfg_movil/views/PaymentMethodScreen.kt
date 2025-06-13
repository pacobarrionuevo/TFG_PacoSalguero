package com.example.tfg_movil.views

import android.widget.Toast
import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.shape.CircleShape
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Delete
import androidx.compose.material.icons.filled.Edit
import androidx.compose.material3.*
import androidx.compose.runtime.*
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
import com.example.tfg_movil.model.paymentMethod.PaymentMethod
import com.example.tfg_movil.viewmodel.ViewModelPaymentMethod

@Composable
fun PaymentMethodScreen(viewModel: ViewModelPaymentMethod) {
    val context = LocalContext.current

    val methods by viewModel.methods.collectAsState()
    val editing by viewModel.editing.collectAsState()
    val error by viewModel.error.collectAsState()

    var method by remember(editing) { mutableStateOf(editing?.method ?: "") }
    var installments by remember(editing) { mutableStateOf(editing?.installments?.toString() ?: "") }
    var firstPaymentDays by remember(editing) { mutableStateOf(editing?.firstPaymentDays?.toString() ?: "") }
    var daysBetweenPayments by remember(editing) { mutableStateOf(editing?.daysBetweenPayments?.toString() ?: "") }

    LaunchedEffect(Unit) {
        viewModel.loadMethods()
    }

    LaunchedEffect(editing) {
        method = editing?.method ?: ""
        installments = editing?.installments?.toString() ?: ""
        firstPaymentDays = editing?.firstPaymentDays?.toString() ?: ""
        daysBetweenPayments = editing?.daysBetweenPayments?.toString() ?: ""
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
                                text = if (editing != null) stringResource(id = R.string.editarMetodoPago)
                                else stringResource(id = R.string.crearMetodoPago),
                                style = MaterialTheme.typography.headlineMedium,
                                color = MaterialTheme.colorScheme.primary,
                                textAlign = TextAlign.Center
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            TextField(
                                value = method,
                                onValueChange = { method = it },
                                label = { Text(stringResource(id = R.string.metodo)) },
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = installments,
                                onValueChange = { installments = it },
                                label = { Text(stringResource(id = R.string.cuotas)) },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = firstPaymentDays,
                                onValueChange = { firstPaymentDays = it },
                                label = { Text(stringResource(id = R.string.diasHastaPrimerPago)) },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            TextField(
                                value = daysBetweenPayments,
                                onValueChange = { daysBetweenPayments = it },
                                label = { Text(stringResource(id = R.string.diasEntrePagos)) },
                                keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Number),
                                modifier = Modifier.fillMaxWidth(),
                                shape = RoundedCornerShape(12.dp)
                            )

                            Spacer(modifier = Modifier.height(24.dp))

                            Button(
                                onClick = {
                                    val installmentsNum = installments.toIntOrNull()
                                    val firstNum = firstPaymentDays.toIntOrNull()
                                    val betweenNum = daysBetweenPayments.toIntOrNull()

                                    if (installmentsNum != null && firstNum != null && betweenNum != null) {
                                        val methodObj = PaymentMethod(
                                            id = editing?.id ?: 0,
                                            method = method,
                                            installments = installmentsNum,
                                            firstPaymentDays = firstNum,
                                            daysBetweenPayments = betweenNum
                                        )

                                        if (editing != null) {
                                            viewModel.update(methodObj)
                                        } else {
                                            viewModel.create(methodObj)
                                        }

                                        method = ""
                                        installments = ""
                                        firstPaymentDays = ""
                                        daysBetweenPayments = ""
                                    } else {
                                        Toast.makeText(context, "Todos los campos deben ser válidos", Toast.LENGTH_SHORT).show()
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
                                    text = if (editing != null) stringResource(id = R.string.GuardarCambios)
                                    else stringResource(id = R.string.crearMetodoPago),
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
                        text = stringResource(id = R.string.listaMetodosPago),
                        style = MaterialTheme.typography.headlineSmall,
                        color = MaterialTheme.colorScheme.onSurface,
                        textAlign = TextAlign.Center
                    )

                    Spacer(modifier = Modifier.height(16.dp))
                }

                items(methods) { item ->
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
                                text = "${stringResource(R.string.metodo)}: ${item.method}",
                                style = MaterialTheme.typography.bodyLarge,
                                color = MaterialTheme.colorScheme.onSurface
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.cuotas)}: ${item.installments}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.diasHastaPrimerPago)}: ${item.firstPaymentDays}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )
                            Spacer(modifier = Modifier.height(4.dp))
                            Text(
                                text = "${stringResource(R.string.diasEntrePagos)}: ${item.daysBetweenPayments}",
                                style = MaterialTheme.typography.bodyMedium,
                                color = MaterialTheme.colorScheme.onSurfaceVariant
                            )

                            Spacer(modifier = Modifier.height(12.dp))

                            Row(
                                horizontalArrangement = Arrangement.End,
                                modifier = Modifier.fillMaxWidth()
                            ) {
                                IconButton(
                                    onClick = { viewModel.startEditing(item) },
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
                                        if (item.id != 0) {
                                            viewModel.delete(item.id)
                                        } else {
                                            Toast.makeText(context, "ID inválido", Toast.LENGTH_SHORT).show()
                                        }
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
