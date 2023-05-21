using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class user_details
{
    public long transid { get; set; }

    public string pan { get; set; } = null!;

    public string fullname { get; set; } = null!;

    public string ucc { get; set; } = null!;

    public string email { get; set; } = null!;

    public string mobile { get; set; } = null!;

    public DateTime? dob { get; set; }

    public DateTime? created_dt { get; set; }

    public string created_by { get; set; } = null!;

    public DateTime? modified_dt { get; set; }

    public string? modified_by { get; set; }

    public string roletype { get; set; } = null!;

    public bool? isactive { get; set; }
}
