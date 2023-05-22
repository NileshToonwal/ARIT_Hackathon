using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class user_details
{
    public long transid { get; set; }

    public string pan { get; set; } 

    public string fullname { get; set; } 

    public string ucc { get; set; } 

    public string email { get; set; } 

    public string mobile { get; set; } 

    public DateTime? dob { get; set; }

    public DateTime? created_dt { get; set; }

    public string created_by { get; set; }

    public DateTime? modified_dt { get; set; }

    public string? modified_by { get; set; }

    public string roletype { get; set; }

    public bool? isactive { get; set; }
}
