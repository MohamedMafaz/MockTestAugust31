package com.example.myapplication

import android.os.Build
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.annotation.RequiresApi
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.selection.toggleable
import androidx.compose.material3.AlertDialog
import androidx.compose.material3.BasicAlertDialog
import androidx.compose.material3.Button
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Surface
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.referentialEqualityPolicy
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.setValue
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.myapplication.ui.theme.MyApplicationTheme
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import java.io.File
import java.nio.file.WatchEvent
import java.time.LocalDateTime
import java.time.temporal.ChronoUnit
import java.util.Calendar
import java.util.Date
import java.util.UUID

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                HellWorld()
            }
        }
    }
}

data class ModelDTO(
    val name: String,
    val time: String
)

@RequiresApi(Build.VERSION_CODES.O)
@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun HellWorld(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var secondsRan by remember { mutableStateOf(0) }
    var isRunning by remember { mutableStateOf(false) }
    var ispopupshown by remember { mutableStateOf(false) }
    var refresh by remember { mutableStateOf(0) }
    var list = remember { mutableStateListOf<ModelDTO>() }
    val endDate = LocalDateTime.of(2026, 9, 15, 0, 0)

    var remaining by remember { mutableStateOf("") }

    LaunchedEffect(Unit) {
        while (true) {
            val now = LocalDateTime.now()

            val seconds = ChronoUnit.SECONDS.between(now, endDate)

            if (seconds <= 0) {
                remaining = "Time's up!"
                break
            }

            val days = seconds / 86400
            val hours = (seconds % 86400) / 3600
            val minutes = (seconds % 3600) / 60
            val secs = seconds % 60

            remaining = "$days Days $hours Hours $minutes Minutes $secs Seconds"

            delay(1000)
        }
    }

    LaunchedEffect(refresh) {
        list.clear()
        var file = File(context.filesDir, "data.json")

        if (!file.exists()) {
            file.writeText("[]")
        }

        list.addAll(Gson().fromJson(file.readText(), object: TypeToken<List<ModelDTO>>(){}.type))
    }

    Column(
        modifier = Modifier.fillMaxSize().padding(30.dp),
        verticalArrangement = Arrangement.Center,
        horizontalAlignment = Alignment.CenterHorizontally
    ) {
        Text("${secondsRan/3600} Hours: ${(secondsRan % 3600)/60} Minutes: ${secondsRan%60} Seconds", fontSize = 20.sp, fontWeight = FontWeight.Bold)

        Text(remaining)
        Button(onClick = {
            isRunning = !isRunning

            if(isRunning){
                scope.launch {
                    while(isRunning){
                        delay(1000)
                        secondsRan++
                    }

                }
            }

        }, modifier = Modifier.height(200.dp).fillMaxWidth()) {
            Text(if(isRunning) "Pause" else "Start")
        }


        Button(onClick = {
            ispopupshown = true
        }, modifier = Modifier.fillMaxWidth()) {
            Text("Stop")
        }

        LazyColumn() {
            items(list){
                Text(it.name)
                Text(it.time)
            }
        }

        if(ispopupshown){
            AlertDialog({
                ispopupshown = false
            }){

                Surface(
                    modifier = Modifier.padding(25.dp)
                ) {
                    Column(

                    ) {
                        Text("Do you want to save it", fontSize = 17.sp)
                        Text("${secondsRan/3600} Hours: ${(secondsRan % 3600)/60} Minutes: ${secondsRan%60} Seconds elapsed", fontSize = 20.sp, fontWeight = FontWeight.Bold)

                        Button(onClick = {
                            ispopupshown = false

                        }, modifier = Modifier.fillMaxWidth()) {
                            Text("Cancel")
                        }

                        Button(onClick = {
                            var a  = ModelDTO(
                                name = UUID.randomUUID().toString(),
                                time = secondsRan.toString()
                            )

                            list.add(a)

            var file = File(context.filesDir, "data.json")
                            file.writeText(Gson().toJson(list))

                            refresh++


                        }, modifier = Modifier.fillMaxWidth()) {
                            Text("Save")
                        }
                    }
                }
            }
        }
    }
}