package com.example.myapplication

import android.annotation.SuppressLint
import android.app.Activity
import android.content.Intent
import android.graphics.BitmapFactory
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.LazyRow
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Card
import androidx.compose.material3.CardColors
import androidx.compose.material3.FilterChip
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
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.runtime.sourceInformationMarkerEnd
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.myapplication.ui.theme.MyApplicationTheme
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.net.URL

class FieldIncidents : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                ViewFieldIncidents()
            }
        }
    }
}

@SuppressLint("CoroutineCreationDuringComposition")
@Composable
private fun ViewFieldIncidents(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var meterlist = remember { mutableStateListOf<Models.GetIncidentDTO>() }
    var filterItems = listOf("Submitted","In-Review","Resolved")
    var searchText by remember { mutableStateOf("") }
    var swipeState = rememberSwipeToDismissBoxState()
    var selectedFilterItems = remember { mutableStateListOf<String>()}
    var refresh by remember { mutableStateOf(0) }

    LaunchedEffect(refresh) {
        meterlist.clear()
        meterlist.addAll(Api.api<List<Models.GetIncidentDTO>>(context,"api/incidents").second!!)

        selectedFilterItems.addAll(filterItems)
    }

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(30.dp)
    ) {
        TextField(searchText, { searchText = it }, label = {
            Text("Search Text")
        }, modifier = Modifier.fillMaxWidth())

        LazyRow(
            modifier = Modifier.fillMaxWidth()
        ) {
            items(filterItems) {
                FilterChip(
                    selected = selectedFilterItems.contains(it),
                    onClick = {
                        if (selectedFilterItems.contains(it)) selectedFilterItems.remove(it)
                        else selectedFilterItems.add(it)
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

                    it.category.lowercase().contains(search)
                }
                .filter {
                    selectedFilterItems.contains(it.status)
                }

            items(filteredItems) { item ->

                ListItem(
                    headlineContent = {
                        Text(item.category)
                    },

                    modifier = Modifier.clickable {
//                            DataStore.selectedMeter = item
//                            context.startActivity(Intent(context, AddSmartMeter::class.java))

                        DataStore.selectedIncidents = item
                        context.startActivity(Intent(context, AdvancedImageInspection::class.java))
                    },
                    supportingContent = {
                        Column() {
                            Text("Timestamp: ${item.createdAt}")
                            Text("Status: ${item.status}")
                        }
                    },

                    leadingContent = {
                        var image by remember { mutableStateOf<ImageBitmap?>(null) }

                        scope.launch {
                            withContext(Dispatchers.IO){
                                image = BitmapFactory.decodeByteArray(URL(item.photoUrl).readBytes(),0, URL(item.photoUrl).readBytes().size).asImageBitmap()
                            }
                        }

                        if(image!=null){
                            Image(image!!,"", modifier = Modifier.width(100.dp).height(100.dp))
                        }
                    },

                    trailingContent = {
//                            Card(
//                                modifier = Modifier.padding(10.dp),
//                                colors = CardColors(
//                                    containerColor = if(item.isActive) Color.Green else Color.Red,
//                                    contentColor = Color.Black,
//                                    disabledContainerColor = Color.Gray,
//                                    disabledContentColor = Color.DarkGray
//                                )
//                            ) {
//                                Text(if(item.isActive) "Active" else "Inactive", modifier = Modifier.padding(10.dp))
//                            }
                    },
                )

            }
        }
    }
}