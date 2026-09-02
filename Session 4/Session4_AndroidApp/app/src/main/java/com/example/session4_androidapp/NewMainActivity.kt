package com.example.session4_androidapp

import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.os.Bundle
import android.provider.ContactsContract
import android.widget.CheckBox
import androidx.activity.ComponentActivity
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.wrapContentWidth
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.Checkbox
import androidx.compose.material3.DatePicker
import androidx.compose.material3.DatePickerDialog
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.Icon
import androidx.compose.material3.ListItem
import androidx.compose.material3.ModalBottomSheet
import androidx.compose.material3.RangeSlider
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TextField
import androidx.compose.material3.TopAppBar
import androidx.compose.material3.adaptive.navigationsuite.NavigationSuiteScaffold
import androidx.compose.material3.rememberDatePickerState
import androidx.compose.material3.rememberModalBottomSheetState
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.currentComposer
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateListOf
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.rememberCoroutineScope
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.graphics.vector.ImageVector
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.example.session4_androidapp.ui.theme.Session4_AndroidAppTheme
import java.io.File
import java.io.FileOutputStream
import java.text.SimpleDateFormat
import java.util.Date
import java.util.Locale

class NewMainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            Session4_AndroidAppTheme {
                Session4_AndroidAppApp()
            }
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@PreviewScreenSizes
@Composable
fun Session4_AndroidAppApp() {

    NavigationSuiteScaffold(
        navigationSuiteItems = {
            AppDestinations.entries.forEach {
                item(
                    icon = {
                        Icon(
                            painterResource(it.icon),
                            contentDescription = it.label
                        )
                    },
                    label = { Text(it.label) },
                    selected = it == DataStore.currentDestination,
                    onClick = { DataStore.currentDestination = it }
                )
            }
        }
    ) {
        Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->

            var context = LocalContext.current
            LaunchedEffect(Unit) {
                DataStore.currentUser = Api.api<Models.LoginResponse>(context,"api/login?email=david.chen%40gmail.com&password=EMS2026Password%21", "POST").second!!
            }

            TopAppBar(title = {
                Text(DataStore.currentUser?.name ?: "")

            },
                actions = {
                    Text("Grid Online")
                }
                )

            Column(
                modifier = Modifier
                    .padding(innerPadding)
                    .padding(30.dp)
            ) {
                if(DataStore.currentDestination == AppDestinations.DASHBOARD){
                    DashboardPage()
                }
                else if(DataStore.currentDestination == AppDestinations.USAGELOGS){
                    UsageLogsSCreen()
                }
                else {
                    ReportScreen()
                }
            }
        }
    }
}

enum class AppDestinations(
    val label: String,
    val icon: Int,
) {
    DASHBOARD("Dashboard", R.drawable.ic_home),
    USAGELOGS("Usage Logs", R.drawable.ic_favorite),
    REPORT("Report", R.drawable.ic_account_box),
}

@Composable
private fun ReportScreen(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var profilePicture by remember { mutableStateOf("") }
    var options = remember {mutableStateListOf<String>() }
    var selectedoption by remember { mutableStateOf<String>("") }
    var optionsExpanded by remember { mutableStateOf<Boolean>(false) }
    var description by remember { mutableStateOf("") }

    LaunchedEffect(Unit) {
        options.clear()
        options.addAll(Api.api<List<String>>(context,"api/incidents").second!!)
        selectedoption = options.first()
    }


    var cameraLauncher = rememberLauncherForActivityResult(
    contract = ActivityResultContracts.TakePicturePreview(),
    onResult = {bitmap ->
        if(bitmap!=null){
            var file = File(context.filesDir,  "IMG_${ System.currentTimeMillis() }.jpg")

            FileOutputStream(file).use { out->
                bitmap.compress(Bitmap.CompressFormat.JPEG,100,out)
            }

            profilePicture = file.name
        }
    }
    )
    var galleryLauncher = rememberLauncherForActivityResult(
    contract = ActivityResultContracts.PickVisualMedia(),
    onResult = { uri->
        if(uri!=null){
            context.contentResolver.openInputStream(uri)?.use { input ->
                var file = File(context.filesDir,"IMG_${System.currentTimeMillis()}.jpg")

                file.outputStream().use { output->
                    input.copyTo(output)
                }

                profilePicture = file.name
            }
        }
    })
    
    Spacer(modifier = Modifier.height(30.dp))

    if(selectedoption!="")
    Column() {
        Text(selectedoption, modifier = Modifier.clickable{
            optionsExpanded = true
        })



        DropdownMenu(optionsExpanded,{
            optionsExpanded = false
        }) {
            options.forEach {
                DropdownMenuItem(
                    text = {
                        Text(it)
                    },
                    onClick = {
                        selectedoption = it
                        optionsExpanded = false
                    },
                )
            }
        }

        if(profilePicture != "")
        Image(BitmapFactory.decodeFile(File(context.filesDir,profilePicture).absolutePath).asImageBitmap(),"")

        Button(onClick = {
            galleryLauncher.launch(PickVisualMediaRequest())
        }) {
            Text("Choose From Gallery")
        }

        Button(onClick = {
            cameraLauncher.launch(null)
        }){
            Text("Camera")
        }
        TextField(description,{
            description = it
        }, label = {
            Text("Description")
        })


        Button(onClick = {

        }) {
            Text("Submit Incident Report")
        }


    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
private fun UsageLogsSCreen(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var list = remember { mutableStateListOf<Models.LogDTO>() }
    var startRate by remember { mutableStateOf(0) }
    var endRate by remember { mutableStateOf(500) }
    var dateRangeOptions = listOf("Today","This Week","Last 30 Days","Custom Range")
    var dateExpanded by remember { mutableStateOf(false) }
    var selectedDateOption by remember { mutableStateOf("") }
    var startDate by remember { mutableStateOf("2000-01-01") }
    var endDate by remember { mutableStateOf("3000-01-01") }
    var startshown by remember { mutableStateOf(false) }
    var endShown by remember { mutableStateOf(false) }
    var startState = rememberDatePickerState()
    var endState = rememberDatePickerState()
    var ispeakhours by remember { mutableStateOf(false) }
    var ispeakoffhours by remember { mutableStateOf(false) }
    var meterList = remember { mutableStateListOf<Models.MeterDTO>() }
    var meterExpanded by remember { mutableStateOf(false) }
    var selectedMeter by remember { mutableStateOf<Models.MeterDTO?>(null) }
    var sortoptiopns = listOf("Sort","Date (Newest First)","Date (Oldest First)","Usage (Highest First)","Usage (Lowest First)")
    var selectedSort by remember { mutableStateOf("") }
    var sortExpanded by remember { mutableStateOf(false) }
    var refresh by remember { mutableStateOf(0) }
    var bottomsheetstate = rememberModalBottomSheetState()

    var isbottomsheetshowing by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) {
        meterList.clear()
        meterList.addAll(Api.api<List<Models.MeterDTO>>(context,"api/meters").second!!)
        meterList.add(0, Models.MeterDTO(0,"All"))
        selectedMeter = meterList.first()
        selectedDateOption = dateRangeOptions.last()
        selectedSort = sortoptiopns.first()
    }

    Spacer(modifier = Modifier.height(30.dp))



    if(selectedMeter!=null){

        LaunchedEffect(refresh) {
            list.clear()
            list.addAll(Api.api<List<Models.LogDTO>>(context,"api/logData?startRate=${startRate}&endRate=${endRate}&deviceId=${selectedMeter!!.meterId}&Sortby=${selectedSort}&dateRange=${selectedDateOption}&startDate=${startDate}&endDate=${endDate}").second!!)
        }

        LazyColumn(
            modifier = Modifier.height(600.dp)
        ) {
            items(list){
                ListItem(
                    headlineContent = {
                        Text(it.meterSerialNumber)
                    },


                    supportingContent = {
                        Column() {
                            Text(it.date)
                            Text("Is Peak: ${it.isPeakHour}")
                        }
                    },

                    trailingContent = {
                        Text(it.unitsKwh.toString())
                    },


                )
            }
        }

        Button(onClick = {
            isbottomsheetshowing = true
        }) {
            Text("Filters")
        }

        if(isbottomsheetshowing){
            ModalBottomSheet(
                onDismissRequest = {
                    isbottomsheetshowing = false
                },

                sheetState = bottomsheetstate,

            ) {
                Text(selectedDateOption, modifier = Modifier.clickable{
                    dateExpanded = true
                })

                DropdownMenu(dateExpanded,{
                    dateExpanded = false
                }) {
                    dateRangeOptions.forEach {
                        DropdownMenuItem(
                            text = {
                                Text(it)
                            },
                            onClick = {
                                selectedDateOption = it
                                dateExpanded = false
                                refresh++
                            },
                        )
                    }
                }


                if(selectedDateOption  == "Custom Range"){
                    Row(

                    ) {

                        Text(startDate, modifier = Modifier.clickable{
                            startshown = true

                        })

                        if(startshown){
                            DatePickerDialog(
                                onDismissRequest = {
                                    startshown = false
                                },
                                confirmButton = {
                                    TextButton(onClick = {
                                        startDate = SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(
                                            Date
                                        (startState.selectedDateMillis!!))

                                        startshown = false

                                        refresh++
                                    }) {
                                        Text("OK")
                                    }
                                },

                            ) {
                                DatePicker(startState)
                            }
                        }



                        Text(endDate, modifier = Modifier.clickable{
                            endShown = true

                        })

                        if(endShown){
                            DatePickerDialog(
                                onDismissRequest = {
                                    endShown = false
                                },
                                confirmButton = {
                                    TextButton(onClick = {
                                        endDate = SimpleDateFormat("yyyy-MM-dd", Locale.getDefault()).format(
                                            Date
                                                (endState.selectedDateMillis!!))

                                        endShown = false

                                        refresh++
                                    }) {
                                        Text("OK")
                                    }
                                },

                                ) {
                                DatePicker(endState)
                            }
                        }
                    }
                }


                Row() {
                    Checkbox(
                        checked = ispeakhours == true,
                        onCheckedChange = {
                            ispeakhours = !ispeakhours
                            refresh++
                        },

                    )

                    Text("Peak Hours")
                }

                Row() {
                    Checkbox(
                        checked = ispeakoffhours == true,
                        onCheckedChange = {
                            ispeakoffhours = !ispeakoffhours
                            refresh++
                        },

                        )

                    Text("Peak-off Hours")
                }


                RangeSlider(
                    value = startRate.toFloat()..endRate.toFloat(),
                    onValueChange = { range ->
                        startRate = range.start.toInt()
                        endRate = range.endInclusive.toInt()

                        refresh++
                    },
                    valueRange = 0f..500f,
                    steps = 499
                )


                Text(selectedMeter!!.meterSerialNumber, modifier = Modifier.clickable{
                    meterExpanded = true
                })

                DropdownMenu(meterExpanded,{
                    meterExpanded = false
                }) {
                    meterList.forEach {
                        DropdownMenuItem(
                            text = {
                                Text(it.meterSerialNumber)
                            },
                            onClick = {
                                selectedMeter = it
                                meterExpanded = false
                                refresh++
                            },
                        )
                    }
                }

                Text(selectedSort, modifier = Modifier.clickable{
                    sortExpanded = true
                })

                DropdownMenu(sortExpanded,{
                    sortExpanded = false
                }) {
                    sortoptiopns.forEach {
                        DropdownMenuItem(
                            text = {
                                Text(it)
                            },
                            onClick = {
                                selectedSort = it
                                sortExpanded = false
                                refresh++
                            },
                        )
                    }
                }



            }
        }
    }



}

@Composable
private fun DashboardPage(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var dashboardData by remember { mutableStateOf<Models.DashboardDTO?>(null) }


    LaunchedEffect(Unit) {
        DataStore.currentUser = Api.api<Models.LoginResponse>(context,"api/login?email=david.chen%40gmail.com&password=EMS2026Password%21", "POST").second!!
    }

    if(DataStore.currentUser !=null){
        LaunchedEffect(Unit) {
            dashboardData = Api.api<Models.DashboardDTO>(context,"api/dashboard").second!!
        }
    }

    Spacer(modifier = Modifier.height(30.dp))


    if(dashboardData!=null){
        Text("Peak Usage: ${(dashboardData!!.peakrate)}")

        Row(
            modifier = Modifier.fillMaxSize()
        ) {

            Card(
                modifier = Modifier.weight(1f)
            ) {
                Text(dashboardData!!.todaysUsage.toString(), fontSize = 20.sp)
                Text("Today's Usage")
            }

            Card(
                modifier = Modifier.weight(1f)
            ) {
                Text(dashboardData!!.estimatedBill.toString(), fontSize = 20.sp)
                Text("Est. Monthly Bill")
            }


            Card(
                modifier = Modifier.weight(1f)
            ) {
                Text(dashboardData!!.netsolarexpoerted.toString(), fontSize = 20.sp)
                Text("Net Solar Exported")
            }
        }
    }


}