using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class user_login
{
    public long transid { get; set; }

    public string pan { get; set; } 

    public string otp { get; set; } 

    public string? device_name { get; set; }

    public DateTime? created_dt { get; set; }

    public string created_by { get; set; } 

    public DateTime? modified_dt { get; set; }

    public string? modified_by { get; set; }

    public string? ip_address { get; set; }

    public DateTime? expiry_dt { get; set; }

    public string roletype { get; set; } 

    public long user_id_ref { get; set; }
}
