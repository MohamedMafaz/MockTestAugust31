package com.example.session4_androidapp

class Models {
    data class LoginResponse(
        val name: String,
        val token: String
    )

    data class DashboardDTO(
        val peakrate: Double,
        val todaysUsage: Double,
        val estimatedBill: Double,
        val netsolarexpoerted: Double
    )

    data class LogDTO(
        val logId: Int,
        val isPeakHour: Boolean,
        val meterSerialNumber: String,
        val date: String,
        val unitsKwh: Double
    )

    data class MeterDTO(
        val meterId: Int,
        val meterSerialNumber: String
    )
}