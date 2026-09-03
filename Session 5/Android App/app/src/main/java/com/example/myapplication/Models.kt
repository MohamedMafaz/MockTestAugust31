package com.example.myapplication

class Models {
    data class GetMeterDTO(
        val meterId: Int,
        val meterSerialNumber: String,
        val customer: String,
        val maxVoltageCapacity: Int,
        val isActive: Boolean,
        val isIndustrial: Boolean,
        val userId: Int,
        val transformerId: Int,
        val dailyUsageLimitKw: Int
    )

    data class DropDownDTO(
        val id: Int,
        val name: String
    )

    data class CreateMeterDTO(
        val meterId: Int = 0,
        val meterSerialNumber: String,
        val transformerId: Int,
        val userId: Int,
        val assignedTechnicianId: Int = 1,
        val tariffPlanId: Int = 1,
        val latitude: Int = 0,
        val longitude: Int = 0,
        val maxVoltageCapacity: Int,
        val dailyUsageLimitKw: Int,
        val isActive: Boolean,
        val isIndustrial: Boolean,
    )

    data class GetIncidentDTO(
        val incidentId: Int,
        val category: String,
        val createdAt: String,
        val status: String,
        val photoUrl: String
    )
}