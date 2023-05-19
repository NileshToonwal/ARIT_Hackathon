using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class UserLogin
{
    public long Transid { get; set; }

    public string Pan { get; set; } = null!;

    public string Otp { get; set; } = null!;

    public string? DeviceName { get; set; }

    public DateTime? CreatedDt { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedDt { get; set; }

    public string? ModifiedBy { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? ExpireyDt { get; set; }

    public string Roletype { get; set; } = null!;
}
