using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Formats.Tar;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class ValuesController : ControllerBase
    {
        GridContext db = new GridContext();

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(string email,string password)
        {

            var user = db.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower() && x.PasswordHash == password);


            if(user == null)
            {
                return Unauthorized("Invalid Credentials");
            }

            var token = new JwtSecurityTokenHandler().WriteToken(
               new JwtSecurityToken(
                   claims:
                   [
                       new Claim(ClaimTypes.Email, user.Email)
                   ],
                   expires: DateTime.UtcNow.AddHours(4),
                   signingCredentials: new SigningCredentials(
                       new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes("HGVJGAVUFDTASDTRvUYJATSCDYUTARSVCDHYARTSVCHD")
                       ),
                       SecurityAlgorithms.HmacSha256
                   )
               )
           );

            return Ok(new
            {
                userid = user.UserId,
                Name = user.FirstName + " " + user.LastName,
                token
            });
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = db.Users.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

            var peakrate = db.SmartMeters.Include(x=>x.User).Include(x=>x.TariffPlan).AsEnumerable()
                .Where(x => x.User.Email == email).Select(x => x.TariffPlan.PeakHourPricePerUnit).FirstOrDefault();

            var todaysUsage = db.EnergyLogs.Include(x=>x.SmartMeter).ThenInclude(x=>x.TariffPlan).Include(x=>x.SmartMeter).ThenInclude(x=>x.User).AsEnumerable()
                .Where(x => DateOnly.FromDateTime(x.Timestamp) == DateOnly.FromDateTime(DateTime.Now) && x.SmartMeter.User.Email == email).Sum(x => x.UnitsKwh);
            var estimatedBill = db.EnergyLogs.Include(x=>x.SmartMeter).ThenInclude(x=>x.User).AsEnumerable()
                .Where(x => DateOnly.FromDateTime(x.Timestamp) == DateOnly.FromDateTime(DateTime.Now) && x.SmartMeter.User.Email == email).Select(x => new
            {
                total = x.IsPeakHour ? x.UnitsKwh * x.SmartMeter.TariffPlan.PeakHourPricePerUnit : x.UnitsKwh * x.SmartMeter.TariffPlan.PricePerUnit
            }).Sum(x => x.total) * 30;

            var netsolarexpoerted = db.EnergyLogs.Include(x=>x.SmartMeter).ThenInclude(x=>x.User).AsEnumerable()
                .Where(x => x.SmartMeter.User.Email == email && x.TransactionTypeId == 2).Sum(x => x.UnitsKwh);
            var now = DateTime.Now;
            var yesterday = now.AddHours(-24);

            var usageoverview = db.EnergyLogs.Include(x=>x.SmartMeter)
                .Where(x =>
                    x.Timestamp >= yesterday &&
                    x.Timestamp <= now &&
                    x.SmartMeter.UserId == user.UserId
                )
                .AsEnumerable()
                .GroupBy(x => x.Timestamp.Hour)
                .Select(x => new
                {
                    Hour = x.Key.ToString(),
                    Total = x.Sum(a => a.UnitsKwh)
                })
                .OrderBy(x => int.Parse(x.Hour))
                .ToList();


            return Ok(new
            {
                peakrate,
                todaysUsage,
                estimatedBill,
                netsolarexpoerted,
                usageoverview,
            });
        }

        [HttpGet("meters")]
        public IActionResult GetMeres()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            return Ok(
                db.SmartMeters.Where(x => x.User.Email.ToLower() == email.ToLower()).Select(x => new
                {
                    x.MeterId,
                    x.MeterSerialNumber
                }).ToList()
                );
        }

        [HttpGet("logData")]
        public IActionResult LgData(
    bool onlyPeakHours = false,
    bool onlyOffPeakHours = false,
    int startRate = 0,
    int endRate = 500,
    int deviceId = 0,
    string? Sortby = "",
    string? dateRange = "",
    DateOnly? startDate = null,
    DateOnly? endDate = null)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = db.Users.FirstOrDefault(x =>
                x.Email.ToLower() == email.ToLower());

            if (user == null)
                return Unauthorized();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var result = db.EnergyLogs
                .Where(x => x.SmartMeter.UserId == user.UserId)
                .Where(x =>
                    (
                        string.IsNullOrEmpty(dateRange)
                        ||
                        (
                            dateRange == "Today"
                                ? DateOnly.FromDateTime(x.Timestamp) == today

                            : dateRange == "This Week"
                                ? DateOnly.FromDateTime(x.Timestamp) <= today
                                  && DateOnly.FromDateTime(x.Timestamp) >= today.AddDays(-6)

                            : dateRange == "Last 30 Days"
                                ? DateOnly.FromDateTime(x.Timestamp) <= today
                                  && DateOnly.FromDateTime(x.Timestamp) >= today.AddDays(-29)

                            : (
                                startDate.HasValue
                                && endDate.HasValue
                                && DateOnly.FromDateTime(x.Timestamp) >= startDate.Value
                                && DateOnly.FromDateTime(x.Timestamp) <= endDate.Value
                            )
                        )
                    )

 && (
    (onlyPeakHours == onlyOffPeakHours)
    || (onlyPeakHours && x.IsPeakHour == true)
    || (onlyOffPeakHours && x.IsPeakHour == false)
)




                    && x.UnitsKwh >= startRate
                    && x.UnitsKwh <= endRate

                    && (deviceId == 0 || x.SmartMeterId == deviceId)
                )
                .Select(x => new
                {
                    x.LogId,
                    x.IsPeakHour,
                    x.SmartMeter.MeterSerialNumber,
                    Date = x.Timestamp.ToString("yyyy-MM-dd"),
                    x.UnitsKwh
                })
                .ToList();


                if(Sortby == "Date (Newest First)")
            {
                result = result.OrderByDescending(x => DateOnly.Parse(x.Date)).ToList();
            }

                if(Sortby == "Date (Oldest First)")
            {
                result = result.OrderBy(x => DateOnly.Parse(x.Date)).ToList();
            }

                if(Sortby == "Usage (Highest First)")
            {
                result = result.OrderByDescending(x => x.UnitsKwh).ToList();
            }

                if(Sortby == "Usage (Lowest First)")
            {
                result = result.OrderBy(x => x.UnitsKwh).ToList();
            }

            return Ok(result);
        }

        [HttpGet("incidents")]
        public IActionResult Incidents()
        {
            return Ok(db.IncidentReports.Select(x=>x.Category).ToList());
        }


        [HttpPost("incidents/upload")]
        public IActionResult Report(
     [FromForm] int userId,
     [FromForm] int? smartMeterId,
     [FromForm] string category,
     [FromForm] string description,
     [FromForm] decimal latitude,
     [FromForm] decimal longitude,
     [FromForm] IFormFile image)
        {
            try
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var folder = Path.Combine("wwwroot", "images");

                Directory.CreateDirectory(folder);

                using var stream = new FileStream(
                    Path.Combine(folder, fileName),
                    FileMode.Create);

                image.CopyTo(stream);

                var report = new IncidentReport
                {
                    UserId = userId,
                    SmartMeterId = smartMeterId,
                    Category = category,
                    Description = description,
                    PhotoUrl = fileName,
                    Latitude = latitude,
                    Longitude = longitude,
                    Status = "Submitted",
                    CreatedAt = DateTime.Now
                };

                db.IncidentReports.Add(report);
                db.SaveChanges();

                return StatusCode(201, new
                {
                    ticket = $"INC-{report.IncidentId:D3}"
                });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
