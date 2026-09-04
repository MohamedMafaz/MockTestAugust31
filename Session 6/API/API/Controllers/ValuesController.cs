using API.Models;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography.Xml;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        GridContext db = new GridContext();

        [HttpGet("queries")]
        public IActionResult GetQueries()
        {
            var query1 = db.SmartMeters.Where(x => x.IsActive == true).Select(x => new
            {
                CustomerName = x.User.FirstName + " " + x.User.LastName,
                x.User.Email,
                x.TariffPlan.PlanName
            }).OrderBy(x => x.CustomerName).ToQueryString();

            var query2 = db.EnergyLogs
    .Where(x =>
        x.Timestamp.Year == 2026 &&
        x.Timestamp.Month == 8)
    .GroupBy(x => x.SmartMeterId)
    .Select(x => new
    {
        MeterId = x.Key,
        TotalEnergy = x.Sum(a => a.UnitsKwh),
        TotalPower = x.Sum(a => a.PowerKw)
    })
    .Where(x => x.TotalEnergy > 10)
    .ToQueryString();


            var query3 = db.Alerts.Where(x => x.Status == "Critical" || x.Status == "Pending").Select(x => new
            {
                FullName = x.SmartMeter.AssignedTechnician.FirstName + " " + x.SmartMeter.AssignedTechnician.LastName,
                x.SmartMeter.MeterSerialNumber,
                x.AlertTitle,
                x.SmartMeter.Transformer.Substation.SubstationName
            }).ToQueryString();

            var query4 = db.EnergyLogs.Where(x => x.IsPeakHour).GroupBy(x => x.SmartMeterId).Select(x => new
            {
                MeterId = x.Key,
                PeakHour = x.Max(a => a.UnitsKwh)
            }).ToQueryString();

            var query5 = db.Users.Select(x => new
            {
                x.UserId,
                CustomerName = x.FirstName + " " + x.LastName,
                Total = x.SmartMeterUsers.Select(a => new { value = a.EnergyLogs.Sum(q => q.UnitsKwh) }).Sum(a => a.value),
                TotalCostFormatted = x.SmartMeterUsers.Select(a => new { value = a.EnergyLogs.Select(c => new { anothervalue = c.IsPeakHour ? c.UnitsKwh * a.TariffPlan.PeakHourPricePerUnit : c.UnitsKwh * a.TariffPlan.PricePerUnit }).Sum(c => c.anothervalue) }).Sum(a => a.value)
            }).ToQueryString();

            return Ok(
                new
                {
                    query1,
                    query2,
                    query3,
                    query4,
                    query5
                }
                );
        }

        [HttpGet("meters")]
        public IActionResult GetMeters()
        {
            var result = db.SmartMeters.Select(x => new
            {
                x.MeterId,
                x.MeterSerialNumber,
                CustomerName = x.User.FirstName + " " + x.User.LastName,
                x.MaxVoltageCapacity,
                x.DailyUsageLimitKw,
                x.IsActive,
                x.Description,
                AnyLogs = x.EnergyLogs.Any()
            }).ToList();

            return Ok(result);
        }

        [HttpGet("transformers")]
        public IActionResult GetTrans()
        {
            return Ok(db.Transformers.Select(x => new
            {
                Id = x.TransformerId,
                Name = $"Transformer {x.TransformerId}"
            }).ToList());
        }


        [HttpGet("customers")]
        public IActionResult Getcustomers()
        {
            return Ok(db.Users.Where(x => x.RoleId == 4).Select(x => new
            {
                Id = x.UserId,
                Name = x.FirstName + " " + x.LastName
            }).ToList());
        }

        [HttpGet("technicians")]
        public IActionResult GetTehc()
        {
            return Ok(db.Users.Where(x => x.RoleId == 3).Select(x => new
            {
                Id = x.UserId,
                Name = x.FirstName + " " + x.LastName
            }).ToList());
        }

        [HttpGet("plans")]
        public IActionResult Getplans()
        {
            return Ok(db.TariffPlans.Select(x=>new
            {
                Id = x.TariffPlanId,
                Name = x.PlanName
            }).ToList());
        }

        [HttpPost("meters")]
        public IActionResult Meters([FromBody] SmartMeter meterId)
        {
            try
            {
                db.SmartMeters.Add(meterId);
                db.SaveChanges();

                return Ok("Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }

        }

        [HttpPut("meters/{id}")]
        public IActionResult UIpdate(int id, [FromBody] SmartMeter meteer)
        {
            try
            {
                db.SmartMeters.Update(meteer);
                db.SaveChanges();

                return Ok("Updated Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet("meterId")]
        //public IActionResult getMeterId()
        //{
        //    var meterid ="EM-"
        //}

        [HttpGet("singleMeter")]
        public IActionResult getMeter(int id)
        {
            return Ok(db.SmartMeters.FirstOrDefault(x=>x.MeterId == id));
        }

        [HttpGet("energyLogs")]
        public IActionResult GetEnergyLogs(int id)
        {
            var enertgylogs = db.EnergyLogs.Where(x => x.SmartMeterId == id).Select(x => new
            {
                x.Timestamp,
                x.UnitsKwh,
                x.Voltage,
                x.CurrentAmps
            }).ToList();

            return Ok(enertgylogs);
        }


        [HttpGet("logspgae")]
        public IActionResult GetLogPageData(int customerId,DateOnly? startDate = null, DateOnly? endDate = null )
        {
            var result = db.EnergyLogs.Where(x =>
            x.SmartMeter.UserId == customerId &&
            (startDate == null || DateOnly.FromDateTime(x.Timestamp) >= startDate) &&
            (endDate == null || DateOnly.FromDateTime(x.Timestamp) <= endDate)
            ).Select(x => new
            {
                x.LogId,
                x.SmartMeter.MeterSerialNumber,
                x.Timestamp,
                x.UnitsKwh,
                x.Voltage,
                x.CurrentAmps,
                x.PowerKw,
                x.IsPeakHour,
                x.SmartMeter.TariffPlan.PeakHourPricePerUnit,
                x.SmartMeter.TariffPlan.PricePerUnit
            }).ToList();


            var totalEnergyConsumed = result.Sum(a => a.UnitsKwh);
            var totalEnergyCost = result.Select(x => new { total = x.IsPeakHour ? x.UnitsKwh * x.PeakHourPricePerUnit : x.UnitsKwh * x.PricePerUnit }).Sum(x => x.total);

            return Ok(new
            {
                totalEnergyConsumed,
                totalEnergyCost,
                result
            });
        }

        [HttpDelete("meters/{id}")]
        public IActionResult DeletMEeter(int id)
        {
            try
            {
                var meter = db.SmartMeters.Include(x => x.IncidentReports).Include(x => x.Alerts).Include(x => x.EnergyLogs).Include(x => x.Invoices).Include(x => x.WorkOrders).ThenInclude(x => x.ComponentReplacementLogs).Include(x => x.MaintenanceRecords)
               .FirstOrDefault(x => x.MeterId == id);

                if (meter != null)
                {
                    if (meter.IncidentReports.Any())
                    {
                        db.IncidentReports.RemoveRange(meter.IncidentReports);
                        db.SaveChanges();
                    }

                    if (meter.Alerts.Any())
                    {
                        db.Alerts.RemoveRange(meter.Alerts);
                        db.SaveChanges();
                    }

                    if (meter.EnergyLogs.Any())
                    {
                        db.EnergyLogs.RemoveRange(meter.EnergyLogs);
                        db.SaveChanges();
                    }


                    if (meter.Invoices.Any())
                    {
                        db.Invoices.RemoveRange(meter.Invoices);
                        db.SaveChanges();
                    }


                    foreach (var item in meter.WorkOrders)
                    {
                        if (item.ComponentReplacementLogs.Any())
                            db.ComponentReplacementLogs.RemoveRange(item.ComponentReplacementLogs);

                    }
                    db.SaveChanges();


                    if (meter.WorkOrders.Any())
                    {
                        db.WorkOrders.RemoveRange(meter.WorkOrders);
                        db.SaveChanges();
                    }

                    if (meter.MaintenanceRecords.Any())
                    {
                        db.MaintenanceRecords.RemoveRange(meter.MaintenanceRecords);
                        db.SaveChanges();

                    }


                    db.SmartMeters.Remove(meter);
                    db.SaveChanges();

                    return Ok("Deleted Successfully");
                }

                return Ok("");

            }
            catch (Exception ex)
            {
                return BadRequest("");
            }
        }
    }
}
