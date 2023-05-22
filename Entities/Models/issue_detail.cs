using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class issue_detail
{
    public long issue_id { get; set; }

    public long user_id_ref { get; set; }

    public string summary { get; set; } 

    public string pan { get; set; } 

    public string fullname { get; set; } 

    public string ucc { get; set; } 

    public string mode { get; set; } 

    public string exchange { get; set; } 

    public string segment { get; set; } 

    public string category { get; set; } 

    public string subcategory { get; set; } 

    public string status { get; set; } 

    public DateTime? targate_date { get; set; }

    public string details { get; set; } 

    public string? filename { get; set; }

    public string? filedata { get; set; }

    public string issue_by { get; set; } 

    public DateTime issue_created_dt { get; set; }

    public string? modified_by { get; set; }

    public DateTime? modified_dt { get; set; }

    public bool? isactive { get; set; }
}
