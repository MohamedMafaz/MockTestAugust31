package com.example.myapplication

import android.content.Intent
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.graphics.Color
import android.media.Image
import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.gestures.detectTapGestures
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.material3.Button
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
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
import androidx.compose.ui.graphics.Canvas
import androidx.compose.ui.graphics.ImageBitmap
import androidx.compose.ui.graphics.Paint
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.input.pointer.pointerInput
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.IntOffset
import androidx.compose.ui.unit.IntSize
import androidx.compose.ui.unit.dp
import com.example.myapplication.ui.theme.MyApplicationTheme
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import java.io.File
import java.net.URL

class AdvancedImageInspection : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            MyApplicationTheme {
                MoreDetailsScreen()
            }
        }
    }
}

@Composable
private fun MoreDetailsScreen(){
    var context = LocalContext.current
    var scope = rememberCoroutineScope()
    var imageBitmap by remember { mutableStateOf<ImageBitmap?>(null) }
    var scribbles = remember { mutableStateListOf<Offset>() }

    LaunchedEffect(Unit) {
        withContext(Dispatchers.IO){
            imageBitmap = BitmapFactory.decodeByteArray(URL(DataStore.selectedIncidents!!.photoUrl).readBytes(),0, URL(
                DataStore.selectedIncidents!!.photoUrl).readBytes().size).asImageBitmap()
        }
    }

    if(imageBitmap!=null){
        Column(
            modifier = Modifier
                .fillMaxSize()
                .padding(30.dp),
            horizontalAlignment = Alignment.CenterHorizontally
        ) {
            Spacer(Modifier.height(40.dp))

            Canvas(modifier = Modifier
                .width(300.dp)
                .height(300.dp)
                .pointerInput(Unit) {
                    detectTapGestures { offset ->
                        scribbles.add(offset)
                    }
                }) {
                drawContext.canvas.drawImageRect(
                    image = imageBitmap!!,
                    srcOffset = IntOffset(0,0),
                    dstSize = IntSize(300.dp.toPx().toInt(),300.dp.toPx().toInt()),
                    paint = Paint(),
                )

                for(i in 1 until scribbles.size){
                    if(i % 2 == 0) continue

                    var firstitem = scribbles[i-1]
                    var secondItem = scribbles[i]

                    drawLine(start = firstitem, end = secondItem, color = androidx.compose.ui.graphics.Color.Red, strokeWidth = 10f)

                    drawCircle(
                        radius = 20f,
                        center = secondItem,
                        color = androidx.compose.ui.graphics.Color.Red
                    )
                }





            }


            Button(onClick = {
                var canvas = android.graphics.Canvas(BitmapFactory.decodeByteArray(URL(DataStore.selectedIncidents!!.photoUrl).readBytes(),0, URL(
                    DataStore.selectedIncidents!!.photoUrl).readBytes().size))

                for(i in 1 until scribbles.size){
                    if(i % 2 == 0) continue

                    var firstitem = scribbles[i-1]
                    var secondItem = scribbles[i]

                    var paint = android.graphics.Paint(

                    )

                    paint.color = Color.RED
                    paint.strokeWidth = 10f

                    canvas.drawLine(firstitem.x, firstitem.y, secondItem.x, secondItem.y, paint)


                }


                var file = File(context.filesDir, "IMG_${System.currentTimeMillis()}.jpeg")

                file.outputStream().use {

                }


            }) {
                Text("Save Annotated Image")
            }

            Button(onClick = {
                scope.launch {
                    var result = Api.api<String>(context,"api/status?id=${DataStore.selectedIncidents!!.incidentId}&status=In-Review","PUT")

                    Toast.makeText(context, result.second!!, Toast.LENGTH_SHORT).show()
                    if(result.first == 200){
                        context.startActivity(Intent(context, FieldIncidents::class.java))
                    }
                }
            }) {
                Text("Mark In-Review")
            }


            Button(onClick = {
                scope.launch {
                    var result = Api.api<String>(context,"api/status?id=${DataStore.selectedIncidents!!.incidentId}&status=Resolved","PUT")

                    Toast.makeText(context, result.second!!, Toast.LENGTH_SHORT).show()
                    if(result.first == 200){
                        context.startActivity(Intent(context, FieldIncidents::class.java))
                    }
                }
            }) {
                Text("Mark Resolved")
            }

            Button(onClick = {
                scope.launch {
                    var result = Api.api<String>(context,"api/dispatch?id=${DataStore.selectedIncidents!!.incidentId}","POST")
                    Toast.makeText(context, result.second!!, Toast.LENGTH_SHORT).show()
                }
            }) {
                Text("Dispatch Technician")
            }
        }
    }

}