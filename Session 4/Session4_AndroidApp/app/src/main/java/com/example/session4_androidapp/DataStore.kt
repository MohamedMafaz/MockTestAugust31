package com.example.session4_androidapp

import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.saveable.rememberSaveable
import androidx.compose.runtime.setValue

object DataStore {
    var currentDestination by mutableStateOf(AppDestinations.DASHBOARD)
    var currentUser  by mutableStateOf<Models.LoginResponse?>(null)

}