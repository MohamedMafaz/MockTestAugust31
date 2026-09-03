package com.example.myapplication

import android.content.Intent
import android.graphics.Paint
import android.os.Bundle
import android.provider.ContactsContract
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.Checkbox
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.material3.rememberSearchBarState
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.myapplication.ui.theme.MyApplicationTheme
import kotlinx.coroutines.launch

class AddSmartMeter : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                AddScreen()
            }
        }
    }
}

@Composable
private fun AddScreen(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var serialNumber by remember { mutableStateOf("") }
    var customerList = remember { mutableStateListOf<Models.DropDownDTO>() }
    var selectedCustoemr by remember { mutableStateOf<Models.DropDownDTO?>(null) }
    var customerExpanded by remember { mutableStateOf(false) }
    var transList = remember { mutableStateListOf<Models.DropDownDTO>() }
    var selectedTranms by remember { mutableStateOf<Models.DropDownDTO?>(null) }
    var transExpanded by remember { mutableStateOf(false) }
    var voltageCapacity by remember { mutableStateOf(0)}
    var dailyUsage by remember { mutableStateOf(0) }
    var isActive by remember { mutableStateOf(false) }
    var isInducstrial by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) {
        dailyUsage = 50
        customerList.clear()
        customerList.addAll(Api.api<List<Models.DropDownDTO>>(context,"api/customers").second!!)
        selectedCustoemr = customerList.first()


        transList.clear()
        transList.addAll(Api.api<List<Models.DropDownDTO>>(context,"api/trans").second!!)
        selectedTranms = transList.first()

        if(DataStore.selectedMeter != null){
            serialNumber = DataStore.selectedMeter!!.meterSerialNumber
            selectedCustoemr = customerList.first { it.id == DataStore.selectedMeter!!.userId }
            selectedTranms = transList.first { it.id == DataStore.selectedMeter!!.transformerId }
            voltageCapacity = DataStore.selectedMeter!!.maxVoltageCapacity
            dailyUsage = DataStore.selectedMeter!!.dailyUsageLimitKw
            isActive = DataStore.selectedMeter!!.isActive
            isInducstrial = DataStore.selectedMeter!!.isIndustrial
        }

    }

    if(selectedCustoemr!=null && selectedTranms!=null){
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(30.dp)
        ) {

            Spacer(Modifier.height(30.dp))
            TextField(serialNumber,{
                serialNumber = it
            }, label = {
                Text("Meter Serial Number")
            })

            MyDropdown(
                value = selectedCustoemr!!.name,
                label = "Customer",
                items = customerList.map { it.name },
                expanded = customerExpanded,
                onExpandedChange = {
                    customerExpanded= it
                },
                onItemSelected = { value ->
                    selectedCustoemr = customerList.first {
                        it.name == value
                    }
                }
            )

            MyDropdown(
                value = selectedTranms!!.name,
                label = "Transformer",
                items = transList.map { it.name },
                expanded = transExpanded,
                onExpandedChange = {
                    transExpanded= it
                },
                onItemSelected = { value ->
                    selectedTranms = transList.first {
                        it.name == value
                    }
                }
            )

            TextField(voltageCapacity.toString(),{
                if(it.all { it.isDigit() } && it.isNotBlank()){
                    voltageCapacity = it.toInt()
                }
            }, label = {
                Text("Max Voltage Capacity")
            }, modifier = Modifier.fillMaxWidth())


            TextField(dailyUsage.toString(),{
                if(it.all { it.isDigit() } && it.isNotBlank()){
                    dailyUsage = it.toInt()
                }
            }, label = {
                Text("Daily Usage Limit")
            }, modifier = Modifier.fillMaxWidth())

            Row(
                verticalAlignment = Alignment.CenterVertically
            ) {
                Checkbox(isActive,{
                    isActive = !isActive
                })

                Text("Is Active")
            }

            Row(
                verticalAlignment = Alignment.CenterVertically
            ) {
                Checkbox(isInducstrial,{
                    isInducstrial = !isInducstrial
                })

                Text("Is Industrial")
            }


            Button(onClick = {
                if(serialNumber.isEmpty() || voltageCapacity < 0){
                    Toast.makeText(context, "All fields are required and volage should be positive", Toast.LENGTH_SHORT).show()
                    return@Button
                }

                var a = Models.CreateMeterDTO(
                    meterId = DataStore.selectedMeter?.meterId ?: 0,
                    meterSerialNumber = serialNumber,
                    transformerId = selectedTranms!!.id,
                    userId = selectedCustoemr!!.id,
                    assignedTechnicianId = 1,
                    tariffPlanId = 1,
                    latitude = 0,
                    longitude = 0,
                    maxVoltageCapacity = voltageCapacity,
                    dailyUsageLimitKw = dailyUsage,
                    isActive = isActive,
                    isIndustrial = isInducstrial
                )

                scope.launch {
                    if(DataStore.selectedMeter == null){
                        var result = Api.api<String>(context,"api/meters","POST",a)
                        Toast.makeText(context, result.second!!, Toast.LENGTH_SHORT).show()

                        if(result.first == 200){
                            context.startActivity(Intent(context, MainActivity::class.java))
                        }

                    }
                    else {
                        var result = Api.api<String>(context,"api/meters/${DataStore.selectedMeter!!.meterId}","PUT",a)
                        Toast.makeText(context, result.second!!, Toast.LENGTH_SHORT).show()

                        if(result.first == 200){
                            context.startActivity(Intent(context, MainActivity::class.java))
                        }
                    }
                }
            }, modifier = Modifier.fillMaxWidth()) {
                Text("Save")
            }

            Button(onClick = {
                context.startActivity(Intent(context, MainActivity::class.java))
            }, modifier = Modifier.fillMaxWidth()) {
                Text("Cancel")
            }
        }
    }

}



@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MyDropdown(
    value: String,
    label: String,
    items: List<String>,
    expanded: Boolean,
    onExpandedChange: (Boolean) -> Unit,
    onItemSelected: (String) -> Unit
) {
    ExposedDropdownMenuBox(
        expanded = expanded,
        onExpandedChange = {
            onExpandedChange(!expanded)
        }
    ) {
        OutlinedTextField(
            value = value,
            onValueChange = {},
            readOnly = true,
            label = { Text(label) },
            trailingIcon = {
                ExposedDropdownMenuDefaults.TrailingIcon(
                    expanded = expanded
                )
            },
            modifier = Modifier
                .menuAnchor()
                .fillMaxWidth()
        )

        ExposedDropdownMenu(
            expanded = expanded,
            onDismissRequest = {
                onExpandedChange(false)
            }
        ) {
            items.forEach {
                DropdownMenuItem(
                    text = {
                        Text(it)
                    },
                    onClick = {
                        onItemSelected(it)
                        onExpandedChange(false)
                    }
                )
            }
        }
    }
}