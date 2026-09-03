package com.example.session4_androidapp

import android.Manifest
import android.app.Activity
import android.app.LocaleManager
import android.content.Context
import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Paint
import android.location.LocationManager
import android.os.Build
import android.os.Bundle
import android.os.LocaleList
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.activity.result.PickVisualMediaRequest
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AppCompatDelegate
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
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
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Translate
import androidx.compose.material3.Button
import androidx.compose.material3.Card
import androidx.compose.material3.Checkbox
import androidx.compose.material3.DatePicker
import androidx.compose.material3.DatePickerDialog
import androidx.compose.material3.DropdownMenu
import androidx.compose.material3.DropdownMenuItem
import androidx.compose.material3.ExperimentalMaterial3Api
import androidx.compose.material3.ExposedDropdownMenuBox
import androidx.compose.material3.ExposedDropdownMenuDefaults
import androidx.compose.material3.Icon
import androidx.compose.material3.ListItem
import androidx.compose.material3.ModalBottomSheet
import androidx.compose.material3.NavigationBarItemDefaults
import androidx.compose.material3.OutlinedTextField
import androidx.compose.material3.RangeSlider
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.material3.TextButton
import androidx.compose.material3.TextField
import androidx.compose.material3.TopAppBar
import androidx.compose.material3.adaptive.navigationsuite.NavigationSuiteDefaults
import androidx.compose.material3.adaptive.navigationsuite.NavigationSuiteScaffold
import androidx.compose.material3.rememberDatePickerState
import androidx.compose.material3.rememberModalBottomSheetState
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
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.graphics.nativeCanvas
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.res.stringResource
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.tooling.preview.PreviewScreenSizes
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.core.content.ContextCompat
import androidx.core.os.LocaleListCompat
import com.example.session4_androidapp.ui.theme.Session4_AndroidAppTheme
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.MultipartBody
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
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
    val itemColors = NavigationSuiteDefaults.itemColors(
        navigationBarItemColors = NavigationBarItemDefaults.colors(
            indicatorColor = Color(android.graphics.Color.parseColor("#FF8C00")),
            selectedIconColor = Color.White,
        )
    )
    NavigationSuiteScaffold(
        navigationSuiteItems = {


            AppDestinations.entries.forEach {
                item(
                    icon = {
                        Icon(
                            painterResource(it.icon),
                            contentDescription = stringResource(it.label)
                        )
                    },
                    label = { Text(stringResource(it.label)) },
                    selected = it == DataStore.currentDestination,
                    onClick = { DataStore.currentDestination = it },
                    colors = itemColors
                )
            }
        }
    ) {
        Scaffold(modifier = Modifier
            .fillMaxSize()
            .padding(10.dp)) { innerPadding ->

            var context = LocalContext.current
            var expanded by remember { mutableStateOf(false) }


            LaunchedEffect(Unit) {
                DataStore.currentUser = Api.api<Models.LoginResponse>(context,"api/login?email=david.chen%40gmail.com&password=EMS2026Password%21", "POST").second!!
            }

            TopAppBar(title = {
                Text("${stringResource(R.string.hello)} ${DataStore.currentUser?.name} \n${stringResource(R.string.account_id)}: ${DataStore.currentUser?.userid}", fontSize = 20.sp)

            },
                actions = {
                    Text(stringResource(R.string.grid_online))

                    Spacer(modifier = Modifier.width(10.dp))

                    Icon(Icons.Default.Translate,"", modifier = Modifier.clickable{
                        expanded = true
                    })

                    DropdownMenu(
                        expanded = expanded,
                        onDismissRequest = {
                            expanded = false
                        }
                    ) {

                        DropdownMenuItem(
                            text = {
                                Text("English")
                            },
                            onClick = {
                                expanded = false

                                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
                                    val localeManager =
                                        context.getSystemService(LocaleManager::class.java)

                                    localeManager.applicationLocales =
                                        LocaleList.forLanguageTags("en")
                                }
                            }
                        )

                        DropdownMenuItem(
                            text = {
                                Text("中文")
                            },
                            onClick = {
                                expanded = false

                                if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
                                    val localeManager =
                                        context.getSystemService(LocaleManager::class.java)

                                    localeManager.applicationLocales =
                                        LocaleList.forLanguageTags("zh-CN")
                                }
                            }
                        )
                    }
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
                else if(DataStore.currentDestination == AppDestinations.REPORT) {
                    ReportScreen()
                }
            }
        }
    }
}

enum class AppDestinations(
    val label: Int,
    val icon: Int,
) {
    DASHBOARD(R.string.dashboard, R.drawable.ic_home),
    USAGELOGS(R.string.usage_logs, R.drawable.ic_favorite),
    REPORT(R.string.report, R.drawable.ic_account_box),
    SMARTMETERS(R.string.smart_meters, R.drawable.ic_account_box),

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
    var meterList = remember { mutableStateListOf<Models.MeterDTO>() }
    var selectedMeter by remember { mutableStateOf<Models.MeterDTO?>(null) }
    var meterExpanded by remember { mutableStateOf(false) }

    LaunchedEffect(Unit) {
        meterList.clear()
        meterList.addAll(Api.api<List<Models.MeterDTO>>(context,"api/meters").second!!)
        selectedMeter = meterList.first()

    }


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
    
    Spacer(modifier = Modifier.height(50.dp))

    if(selectedoption!="" && selectedMeter!=null)
    Column() {
        MyDropdown(
            value = selectedMeter!!.meterSerialNumber,
            label = stringResource(R.string.smart_meter),
            items = meterList.map { it.meterSerialNumber },
            expanded = meterExpanded,
            onExpandedChange = {
                meterExpanded = it
            },
            onItemSelected = { value ->
                selectedMeter = meterList.first {
                    it.meterSerialNumber == value
                }
            }
        )



        MyDropdown(
            value = selectedoption,
            label = stringResource(R.string.incident_type),
            items = options,
            expanded = optionsExpanded,
            onExpandedChange = {
                optionsExpanded = it
            },
            onItemSelected = {
                selectedoption = it
            }
        )

        if(profilePicture != ""){
            Spacer(Modifier.height(10.dp))
            Row(
                modifier = Modifier.fillMaxWidth()
            ) {


                Image(
                    BitmapFactory.decodeFile(File(context.filesDir, profilePicture).absolutePath)
                        .asImageBitmap(), "",
                    modifier = Modifier
                        .height(200.dp)
                        .width(200.dp)
                )

                Button(onClick = {
                    profilePicture = ""
                }) {
                    Text(stringResource(R.string.remove))
                }
            }

        }

        Spacer(Modifier.height(10.dp))
        Row(
            modifier = Modifier.fillMaxWidth()
        ) {

            Button(onClick = {
                galleryLauncher.launch(PickVisualMediaRequest())
            }, modifier = Modifier.weight(1f)) {
                Text(stringResource(R.string.gallery))
            }

            Spacer(Modifier.width(10.dp))
            Button(onClick = {
                cameraLauncher.launch(null)
            }, modifier = Modifier.weight(1f)){
                Text(stringResource(R.string.camera))
            }
        }

        Spacer(Modifier.height(10.dp))


        TextField(description,{
            description = it
        }, label = {
            Text(stringResource(R.string.description))
        }, modifier = Modifier.fillMaxWidth(), minLines = 3)
        Spacer(Modifier.height(10.dp))

        Button(onClick = {

            if (description.isBlank() || profilePicture == "") {
                Toast.makeText(context, "All fields are required", Toast.LENGTH_SHORT).show()
                return@Button
            }

            if(description.length > 300){
                Toast.makeText(context, "Description should be 300 charachters maximum", Toast.LENGTH_SHORT).show()
                return@Button
            }

            if (ContextCompat.checkSelfPermission(
                    context,
                    Manifest.permission.ACCESS_FINE_LOCATION
                ) != PackageManager.PERMISSION_GRANTED
            ) {
                (context as Activity).requestPermissions(
                    arrayOf(Manifest.permission.ACCESS_FINE_LOCATION),
                    100
                )
                return@Button
            }

            val locationManager =
                context.getSystemService(Context.LOCATION_SERVICE) as LocationManager

            val location =
                locationManager.getLastKnownLocation(LocationManager.GPS_PROVIDER)

            if (location == null) {
                Toast.makeText(context, "Unable to get location", Toast.LENGTH_SHORT).show()
                return@Button
            }


            scope.launch {

                val result = withContext(Dispatchers.IO) {

                    val body = MultipartBody.Builder()
                        .setType(MultipartBody.FORM)
                        .addFormDataPart(
                            "userId",
                            DataStore.currentUser!!.userid.toString()
                        )
                        .addFormDataPart(
                            "smartMeterId",
                            selectedMeter!!.meterId.toString()
                        )
                        .addFormDataPart(
                            "category",
                            selectedoption
                        )
                        .addFormDataPart(
                            "description",
                            description
                        )
                        .addFormDataPart(
                            "latitude",
                            location.latitude.toString()
                        )
                        .addFormDataPart(
                            "longitude",
                            location.longitude.toString()
                        )
                        .addFormDataPart(
                            "image",
                            profilePicture,
                            File(context.filesDir, profilePicture)
                                .readBytes()
                                .toRequestBody("image/jpeg".toMediaType())
                        )
                        .build()

                    val request = Request.Builder()
                        .url(Api.baseUrl + "api/incidents/upload")
                        .post(body)
                        .addHeader(
                            "Authorization",
                            "Bearer ${DataStore.currentUser!!.token}"
                        )
                        .build()

                    val response = Api.client.newCall(request).execute()

                    response.code to response.body?.string()
                }

                if (result.first == 201) {
                    Toast.makeText(
                        context,
                        "Saved Successfully",
                        Toast.LENGTH_SHORT
                    ).show()

                    DataStore.currentDestination = AppDestinations.DASHBOARD
                }
                else {
                    Toast.makeText(context, result.toString(), Toast.LENGTH_SHORT).show()
                }
            }

        }, modifier = Modifier.fillMaxWidth()) {
            Text(stringResource(R.string.submit_incident_report))
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
            list.addAll(Api.api<List<Models.LogDTO>>(context,"api/logData?onlyPeakHours=${ispeakhours}&onlyOffPeakHours=${ispeakoffhours}&startRate=${startRate}&endRate=${endRate}&deviceId=${selectedMeter!!.meterId}&Sortby=${selectedSort}&dateRange=${selectedDateOption}&startDate=${startDate}&endDate=${endDate}").second!!)
        }

        if(list.isEmpty()){
            Text(stringResource(R.string.no_consumption_logs))
        }
        LazyColumn(
            modifier = Modifier.height(500.dp)
        ) {
            items(list){
                ListItem(
                    headlineContent = {
                        Text(it.meterSerialNumber)
                    },


                    supportingContent = {
                        Column() {
                            Text(it.date)
                            Text("${stringResource(R.string.is_peak)} ${it.isPeakHour}")
                        }
                    },

                    trailingContent = {
                        Text(it.unitsKwh.toString())
                    },


                )
            }
        }

        Spacer(Modifier.height(10.dp))
        Button(onClick = {
            isbottomsheetshowing = true
        }, modifier = Modifier.fillMaxWidth()) {
            Text("Filters")
        }

        if(isbottomsheetshowing){
            ModalBottomSheet(
                onDismissRequest = {
                    isbottomsheetshowing = false
                },

                sheetState = bottomsheetstate,

            ) {
                Column(
                    modifier = Modifier.padding(35.dp)
                ) {
                    MyDropdown(
                        value = selectedDateOption,
                        label = "Date Option",
                        items = dateRangeOptions,
                        expanded = dateExpanded,
                        onExpandedChange = {
                            dateExpanded = it
                        },
                        onItemSelected = { value ->
                            selectedDateOption = dateRangeOptions.first {
                                it == value
                            }
                        }
                    )

                    Spacer(modifier = Modifier.height(15.dp))


                    if(selectedDateOption  == "Custom Range"){
                        Row(
                            modifier = Modifier.fillMaxWidth()
                        ) {

                            Text(startDate, modifier = Modifier
                                .clickable {
                                    startshown = true

                                }
                                .weight(1f))

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

                                        }) {
                                            Text(stringResource(R.string.ok))
                                        }
                                    },

                                    ) {
                                    DatePicker(startState)
                                }
                            }



                            Text(endDate, modifier = Modifier
                                .clickable {
                                    endShown = true

                                }
                                .weight(1f))

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

                                        }) {
                                            Text(stringResource(R.string.ok))
                                        }
                                    },

                                    ) {
                                    DatePicker(endState)
                                }
                            }
                        }
                    }
                    Spacer(modifier = Modifier.height(15.dp))


                    Row(
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Checkbox(
                            checked = ispeakhours == true,
                            onCheckedChange = {
                                ispeakhours = !ispeakhours
                            },

                            )

                        Text(stringResource(R.string.peak_hours))
                    }

                    Row(
                        verticalAlignment = Alignment.CenterVertically
                    ) {
                        Checkbox(
                            checked = ispeakoffhours == true,
                            onCheckedChange = {
                                ispeakoffhours = !ispeakoffhours
                            },

                            )

                        Text(stringResource(R.string.peak_off_hours))
                    }

                    Row(
                        modifier = Modifier.fillMaxWidth(),
                        horizontalArrangement = Arrangement.SpaceBetween
                    ) {
                        Text(startRate.toString())
                        Text(endRate.toString())
                    }


                    RangeSlider(
                        value = startRate.toFloat()..endRate.toFloat(),
                        onValueChange = { range ->
                            startRate = range.start.toInt()
                            endRate = range.endInclusive.toInt()

                        },
                        valueRange = 0f..500f,
                        steps = 499
                    )
                    Spacer(modifier = Modifier.height(15.dp))


                    MyDropdown(
                        value = selectedMeter!!.meterSerialNumber,
                        label = stringResource(R.string.smart_meter),
                        items = meterList.map { it.meterSerialNumber },
                        expanded = meterExpanded,
                        onExpandedChange = {
                            meterExpanded = it
                        },
                        onItemSelected = { value ->
                            selectedMeter = meterList.first {
                                it.meterSerialNumber == value
                            }
                        }
                    )

                    Spacer(modifier = Modifier.height(15.dp))


                    MyDropdown(
                        value = selectedSort,
                        label = "Sort",
                        items = sortoptiopns,
                        expanded = sortExpanded,
                        onExpandedChange = {
                            sortExpanded = it
                        },
                        onItemSelected = { value ->
                            selectedSort = sortoptiopns.first {
                                it == value
                            }
                        }
                    )

                    Spacer(modifier = Modifier.height(15.dp))


                    Row(
                        modifier = Modifier.fillMaxWidth()
                    ) {
                        Button(onClick = {
                            selectedDateOption = dateRangeOptions.last()
                            ispeakoffhours = false
                            ispeakhours = false
                            startRate = 0
                            endRate = 500
                            selectedMeter = meterList.first()
                            selectedSort = sortoptiopns.first()
                            startDate = "2000-01-01"
                            endDate = "3000-01-01"
                            refresh++

                        }, modifier = Modifier.weight(1f)) {
                            Text(stringResource(R.string.reset_filter))
                        }

                        Spacer(modifier = Modifier.width(5.dp))
                        Button(onClick = {
                            refresh++
                        },modifier = Modifier.weight(1f)) {
                            Text(stringResource(R.string.apply))
                        }
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

    Spacer(modifier = Modifier.height(50.dp))


    if(dashboardData!=null){
        Text("Peak Usage: ${(dashboardData!!.peakrate)}")

        Row(
            modifier = Modifier.fillMaxWidth()
        ) {

            Card(
                modifier = Modifier
                    .weight(1f)
                    .padding(10.dp)
            ) {
                Column(
                    modifier = Modifier.padding(15.dp)
                ) {
                    Text(dashboardData!!.todaysUsage.toString(), fontSize = 20.sp)
                    Text(stringResource(R.string.todays_usage))
                }

            }

            Card(
                modifier = Modifier
                    .weight(1f)
                    .padding(10.dp)
            ) {
                Column(
                    modifier = Modifier.padding(15.dp)
                ) {
                    Text(dashboardData!!.estimatedBill.toString(), fontSize = 20.sp)
                    Text(stringResource(R.string.estimated_monthly_bill))
                }

            }


            Card(
                modifier = Modifier
                    .weight(1f)
                    .padding(10.dp)
            ) {
                Column(
                    modifier = Modifier.padding(15.dp)
                ) {
                    Text(dashboardData!!.netsolarexpoerted.toString(), fontSize = 20.sp)
                    Text(stringResource(R.string.net_solar_exported))
                }
            }
        }

        Spacer(Modifier.height(50.dp))
        Text(stringResource(R.string.usage_overview_chart), fontSize = 20.sp, fontWeight = FontWeight.Bold)
        Spacer(Modifier.height(50.dp))


        Canvas(
            modifier = Modifier
                .fillMaxWidth()
                .height(300.dp)
        ) {
            val data = dashboardData!!.usageoverview
            val max = data.maxOf { it.total }

            val graphWidth = size.width - 70f
            val graphHeight = size.height - 60f

            drawLine(
                color = Color.Black,
                start = Offset(50f, 0f),
                end = Offset(50f, graphHeight)
            )

            drawLine(
                color = Color.Black,
                start = Offset(50f, graphHeight),
                end = Offset(size.width, graphHeight)
            )

            for (i in 1 until data.size) {

                val x1 = 50f + (graphWidth / (data.size - 1)) * (i - 1)
                val x2 = 50f + (graphWidth / (data.size - 1)) * i

                val y1 = graphHeight - (data[i - 1].total / max * graphHeight)
                val y2 = graphHeight - (data[i].total / max * graphHeight)

                drawLine(
                    color = Color(android.graphics.Color.parseColor("#FF8C00")),
                    start = Offset(x1, y1.toFloat()),
                    end = Offset(x2, y2.toFloat()),
                    strokeWidth = 5f
                )

                drawContext.canvas.nativeCanvas.drawText(
                    data[i].hour.toString(),
                    x2,
                    graphHeight + 35f,
                    Paint().apply {
                        textSize = 30f
                        color = android.graphics.Color.BLACK
                    }
                )
            }

            drawContext.canvas.nativeCanvas.drawText(
                data[0].hour.toString(),
                50f,
                graphHeight + 35f,
                Paint().apply {
                    textSize = 30f
                    color = android.graphics.Color.BLACK
                }
            )

            for (i in 0..5) {
                val value = max * i / 5f
                val y = graphHeight - (graphHeight * i / 5f)

                drawContext.canvas.nativeCanvas.drawText(
                    "%.1f".format(value),
                    5f,
                    y,
                    Paint().apply {
                        textSize = 25f
                        color = android.graphics.Color.BLACK
                    }
                )
            }

            drawContext.canvas.nativeCanvas.drawText(
                "Usage (kWh)",
                5f,

                -40f,
                Paint().apply {
                    textSize = 30f
                    color = android.graphics.Color.BLACK
                }
            )

            drawContext.canvas.nativeCanvas.drawText(
                "Time",
                size.width - 60f,
                size.height + 15f,
                Paint().apply {
                    textSize = 30f
                    color = android.graphics.Color.BLACK
                }
            )
        }
    }


}