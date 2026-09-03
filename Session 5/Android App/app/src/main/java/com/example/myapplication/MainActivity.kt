package com.example.myapplication

import android.app.Activity
import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Adb
import androidx.compose.material.icons.filled.Add
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.CardColors
import androidx.compose.material3.FilterChip
import androidx.compose.material3.FloatingActionButton
import androidx.compose.material3.Icon
import androidx.compose.material3.ListItem
import androidx.compose.material3.Scaffold
import androidx.compose.material3.SwipeToDismissBox
import androidx.compose.material3.Text
import androidx.compose.material3.TextField
import androidx.compose.material3.rememberSwipeToDismissBoxState
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.referentialEqualityPolicy
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.myapplication.ui.theme.MyApplicationTheme
import kotlinx.coroutines.launch

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                FirstScreen()
            }
        }
    }
}

@Composable
private fun FirstScreen(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var meterlist = remember { mutableStateListOf<Models.GetMeterDTO>() }
    var filterItems = listOf("All","Active Only","Industrial Only")
    var searchText by remember { mutableStateOf("") }
    var selectedFilterItems = remember { mutableStateListOf<String>()}
    var refresh by remember { mutableStateOf(0) }

    LaunchedEffect(refresh) {
        meterlist.clear()
        meterlist.addAll(Api.api<List<Models.GetMeterDTO>>(context,"api/meters").second!!)
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(30.dp)
    ) {
        TextField(searchText,{searchText = it}, label = {
            Text("Search Text")
        }, modifier = Modifier.fillMaxWidth())

        LazyRow(
            modifier = Modifier.fillMaxWidth()
        ) {
            items(filterItems){
                FilterChip(
                    selected = selectedFilterItems.contains(it),
                    onClick = {
                        if(selectedFilterItems.contains(it)) selectedFilterItems.remove(it)
                        else selectedFilterItems.add(it)

                        if(filterItems.filter { it != "All" }.intersect(selectedFilterItems.filter { it!="All" }).any()){
                            selectedFilterItems.add("All")
                        }
                    },
                    label = {
                        Text(it)
                    },
                    modifier = Modifier.padding(10.dp)
                )
            }
        }


        LazyColumn(
            modifier = Modifier.weight(1f)
        ) {
            val filteredItems = meterlist
                .filter {
                    val search = searchText.lowercase()

                    it.meterSerialNumber.lowercase().contains(search) ||
                            it.customer.lowercase().contains(search)
                }
                .filter {
                    if (selectedFilterItems.contains("All")) {
                        true
                    } else {
                        (!selectedFilterItems.contains("Active Only") || it.isActive) &&
                                (!selectedFilterItems.contains("Industrial Only") || it.isIndustrial)
                    }
                }

            items(filteredItems){ item->

                var swipeState = rememberSwipeToDismissBoxState()



                SwipeToDismissBox(
                    state = swipeState,
                    backgroundContent = {

                    },

                    onDismiss = {
                        scope.launch {
                            var result = Api.api<String>(context,"api/meters/${item.meterId}","DELETE")

                            if(result.first == 200){
                                (context as Activity).runOnUiThread {
                                    Toast.makeText(context, result.second, Toast.LENGTH_SHORT).show()
                                }
                                refresh++
                            }


                        }
                    }
                ) {
                    ListItem(
                        headlineContent = {
                            Text(item.meterSerialNumber)
                        },

                        modifier = Modifier.clickable{
                            DataStore.selectedMeter = item
                            context.startActivity(Intent(context, AddSmartMeter::class.java))

                        },
                        supportingContent = {
                            Column() {
                                Text("Customer: ${item.customer}")
                                Text("Voltage Capacity: ${item.maxVoltageCapacity}")
                            }
                        },

                        trailingContent = {
                            Card(
                                modifier = Modifier.padding(10.dp),
                                colors = CardColors(
                                    containerColor = if(item.isActive) Color.Green else Color.Red,
                                    contentColor = Color.Black,
                                    disabledContainerColor = Color.Gray,
                                    disabledContentColor = Color.DarkGray
                                )
                            ) {
                                Text(if(item.isActive) "Active" else "Inactive", modifier = Modifier.padding(10.dp))
                            }
                        },
                    )
                }
            }
        }

        Button(onClick = {
                context.startActivity(Intent(context, FieldIncidents::class.java))
        }) {
            Text("View Field Incidents")
        }

        FloatingActionButton(onClick = {
            DataStore.selectedMeter = null
            context.startActivity(Intent(context, AddSmartMeter::class.java))
        }) {
            Icon(Icons.Default.Add,"")
        }
    }
}