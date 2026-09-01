using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
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
        EnergyContext db = new EnergyContext();

        public static List<(string, int)> invalid = new List<(string, int)>();

        public class LoginRequest
        {
            public string username { get; set; }

            public string password { get; set; }
        }


        [HttpPost("auth/login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = db.Users
                .Include(x => x.Role)
                .FirstOrDefault(x => x.Username == request.username);

            if (user == null)
                return Unauthorized();

            if (!(bool)user.IsActive)
                return Unauthorized();

            if (user.Password != request.password)
            {
                var index = invalid.FindIndex(x => x.Item1 == request.username);

                if (index == -1)
                {
                    invalid.Add((request.username, 1));
                }
                else
                {
                    var attempts = invalid[index].Item2 + 1;
                    invalid[index] = (request.username, attempts);

                    if (attempts >= 5)
                    {
                        user.IsActive = false;
                        db.SaveChanges();
                    }
                }

                return Unauthorized();
            }

            var token = new JwtSecurityTokenHandler().WriteToken(
                new JwtSecurityToken(
                    claims:
                    [
                        new Claim(ClaimTypes.Email, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
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
            invalid.RemoveAll(x => x.Item1 == request.username);
 
            return Ok(token);
        }



        [HttpGet("meters")]
        public IActionResult GetMeters(int page, int pageSize, int facilityId, bool isActive)
        {
            var result = db.EnergyMeters.Where(x => x.FacilityId == facilityId && x.IsActive == isActive).ToList().Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(result);
        }

        [HttpGet("meters/{id}")]
        public IActionResult GetSinglemetre(int id)
        {
            var meters = db.EnergyMeters.FirstOrDefault(x => x.MeterId == id);

            if (meters == null)
            {
                return NotFound();
            }

            return Ok(meters);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("meters")]
        public IActionResult PostMeter([FromBody] EnergyMeter meter)
        {
            if (db.EnergyMeters.Any(x => x.MeterSerialNumber.ToLower() == meter.MeterSerialNumber.ToLower()))
            {
                return Conflict();
            }

            if (meter.MaxVoltageCapacity < 0)
            {
                return BadRequest();
            }

            db.EnergyMeters.Add(meter);
            db.SaveChanges();

            return Created();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("meters/{id}")]
        public IActionResult UpdateMeters(int id, [FromBody] EnergyMeter meter)
        {
            if (id != meter.MeterId)
            {
                return BadRequest();
            }

            var en = db.EnergyMeters.AsNoTracking()
                .FirstOrDefault(x => x.MeterId == id);

            if (en == null)
            {
                return NotFound();
            }

            db.EnergyMeters.Update(meter);
            db.SaveChanges();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("meters/{id}")]
        public IActionResult DeleteMeter(int id)
        {
            var en = db.EnergyMeters.Include(x => x.Alerts)
                .FirstOrDefault(x => x.MeterId == id);

            if (en == null)
            {
                return NotFound();
            }

            if (en.Alerts.Any(x => x.Status == "Pending"))
            {
                return Conflict();
            }

            en.IsActive = false;
            db.SaveChanges();

            return Ok();
        }

        [HttpGet("facilities")]
        public IActionResult GetFacilities()
        {
            return Ok(db.Facilities.ToList());
        }

        [HttpPost("logs")]
        public IActionResult PostLogs([FromBody] ConsumptionLog conlog)
        {
            if(conlog.Timestamp > DateTime.Now)
            {
                return UnprocessableEntity();
            }

            if(conlog.PowerKw < 0)
            {
                return BadRequest();
            }

            db.ConsumptionLogs.Add(conlog);
            db.SaveChanges();

            return Created();
        }


        [HttpGet("alerts")]
        public IActionResult GetALerts(string severity)
        {
            if(severity.ToLower() != "Low".ToLower() &&  severity.ToLower() != "Medium".ToLower() && severity.ToLower() != "Critical".ToLower())
            {
                return BadRequest();
            }

            var result = db.Alerts.Where(x => x.Severity.ToLower() == severity.ToLower()).ToList();

            return Ok(result);
        }

        [HttpPut("alerts/{id}/status")]
        public IActionResult UPdateStatus(string status, int id)
        {
            var alert = db.Alerts.FirstOrDefault(x => x.AlertId == id);

            if (status.ToLower() != "Pending".ToLower() && status.ToLower() != "Resolved".ToLower() && status.ToLower() != "Dismissed".ToLower())
                return BadRequest();

            if(alert == null)
            {
                return NotFound();
            }

            if(alert.Status == "Dismissed" && status == "Pending" && !User.IsInRole("Admin"))
            {
                return BadRequest();
            }

            alert.Status = status;
            db.SaveChanges();

            return Ok();

        }
    }
}
