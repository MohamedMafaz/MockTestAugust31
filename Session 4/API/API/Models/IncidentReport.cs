using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API.Models;

public partial class IncidentReport
{
    public int IncidentId { get; set; }

    public int UserId { get; set; }

    public int? SmartMeterId { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string PhotoUrl { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public virtual SmartMeter? SmartMeter { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; } = null;
}
