package com.example.session4_androidapp

import android.app.Activity
import android.content.Context
import android.util.Log
import android.widget.Toast
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody

object Api {
    public var baseUrl = "http://10.40.0.84:5036/"
    public var client = OkHttpClient()
    public var gson = Gson()

    suspend inline fun<reified T> api(
        context: Context,
        endpoint: String,
        method: String = "GET",
        body: Any? = null
    ) : Pair<Int, T?> = withContext(Dispatchers.IO){
        Log.d("API_ERROR",baseUrl+endpoint)

        var request = Request.Builder().url(baseUrl+endpoint).method(method,
            if(method == "GET" || method == "DELETE") null
            else gson.toJson(body).toRequestBody("application/json".toMediaType())
            )

        if(!endpoint.contains("login")){
            request.addHeader("Authorization", "Bearer ${DataStore.currentUser!!.token}")
        }

        var response = client.newCall(request.build()).execute()

        if(!response.isSuccessful){
            (context as Activity).runOnUiThread {
                Toast.makeText(context, response.body?.string(), Toast.LENGTH_SHORT).show()
            }
        }

        var output = response.body?.string()

        if(T::class == Unit::class || T::class == String::class || T::class == Int::class){
            return@withContext response.code to output as T

        }

        var type = object: TypeToken<T>(){}.type
        return@withContext response.code to gson.fromJson(output, type)
    }
}