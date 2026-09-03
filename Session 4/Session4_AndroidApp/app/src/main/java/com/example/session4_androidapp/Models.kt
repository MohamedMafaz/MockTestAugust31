package com.example.session4_androidapp

class Models {
    data class LoginResponse(
        val name: String,
        val token: String,
        val userid: Int
    )

    data class DashboardDTO(
        val peakrate: Double,
        val todaysUsage: Double,
        val estimatedBill: Double,
        val netsolarexpoerted: Double,
        val usageoverview: List<UsageOverviewDTO>
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

    data class ReportDTO(
        val userId: Int,
        val category: String,
        val description: String,
        val photoUrl: String,
        val latitude: Double,
        val longitude: Double,
        val status: String
    )

    data class UsageOverviewDTO(
        val hour: String,
        val total: Double
    )
}