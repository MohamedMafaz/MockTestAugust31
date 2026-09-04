using System;
using System.Collections.Generic;

namespace API.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public string? InvoiceUrl { get; set; }

    public int SmartMeterId { get; set; }

    public int UserId { get; set; }

    public DateOnly BillingPeriodStart { get; set; }

    public DateOnly BillingPeriodEnd { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual SmartMeter SmartMeter { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
