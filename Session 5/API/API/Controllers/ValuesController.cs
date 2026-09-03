using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        GridContext db = new GridContext();

        [HttpGet("meters")]
        public IActionResult GetMeters()
        {
            return Ok(db.SmartMeters.Select(x => new
            {
                x.MeterId,
                x.MeterSerialNumber,
                Customer = x.User.FirstName + " " + x.User.LastName,
                x.MaxVoltageCapacity,
                x.IsActive,
                x.IsIndustrial,
                x.UserId,
                x.TransformerId,
                x.DailyUsageLimitKw
            }).ToList());
        }

        [HttpDelete("meters/{id}")]
        public IActionResult DeletMEeter(int id)
        {
            try
            {
                var meter = db.SmartMeters.Include(x => x.IncidentReports).Include(x => x.Alerts).Include(x => x.EnergyLogs).Include(x => x.Invoices).Include(x => x.WorkOrders).ThenInclude(x => x.ComponentReplacementLogs).Include(x => x.MaintenanceRecords)
               .FirstOrDefault(x => x.MeterId == id);

                if(meter != null)
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
            catch(Exception ex)
            {
                return BadRequest("");
            }
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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("customers")]
        public IActionResult GetCustomer()
        {
            return Ok(db.Users.Where(x => x.UserId == 4).Select(x => new { 
                Id = x.UserId,
                Name = x.FirstName + " " + x.LastName
            }));
            
        }


        [HttpGet("trans")]
        public IActionResult GHetTramnsformers()
        {
            return Ok(db.Transformers.Select(x => new
            {
                Id = x.TransformerId,
                Name = $"Transformer {x.TransformerId}"
            }).ToList());
        }

        [HttpGet("incidents")]
        public IActionResult GetIncidents()
        {
            var result = db.IncidentReports.Select(x => new
            {
                x.IncidentId,
                x.Category,
                x.CreatedAt,
                x.Status,
                x.PhotoUrl
            }).ToList();

            return Ok(result);
        }

        [HttpPut("status")]
        public IActionResult UpdateStatus(int id, string status)
        {
            var incident = db.IncidentReports.FirstOrDefault(x => x.IncidentId == id);
            incident.Status = status;
            db.SaveChanges();

            return Ok("Status Updated Successfully");
        }

        [HttpPost("dispatch")]
        public IActionResult Dispatch(int id)
        {

            var incident = db.IncidentReports.FirstOrDefault(x => x.IncidentId == id);

            if(db.WorkOrders.Any(x=>x.Status != "Completed" && x.SmartMeterId == (int)incident.SmartMeterId))
            {
                return BadRequest("Technician Dispatched Already");
            }

            var order = new WorkOrder
            {
                SmartMeterId = (int)incident.SmartMeterId,
                TechnicianId = 1,
                CreatedById = 1,
                Description = "",
                Status = "Assigned",
                CreatedAt = DateTime.Now
            };

            db.WorkOrders.Add(order);
            db.SaveChanges();

            return Ok("Technician Dispatched Successfully");
        }
    }
}
