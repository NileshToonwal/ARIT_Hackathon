using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class issue_detail
{
    public long issue_id { get; set; }

    public long user_id_ref { get; set; }

    public string summary { get; set; } = null!;

    public string pan { get; set; } = null!;

    public string fullname { get; set; } = null!;

    public string ucc { get; set; } = null!;

    public string mode { get; set; } = null!;

    public string exchange { get; set; } = null!;

    public string segment { get; set; } = null!;

    public string category { get; set; } = null!;

    public string subcategory { get; set; } = null!;

    public string status { get; set; } = null!;

    public DateTime? targate_date { get; set; }

    public string details { get; set; } = null!;

    public string? filename { get; set; }

    public string? filedata { get; set; }

    public string issue_by { get; set; } = null!;

    public DateTime issue_created_dt { get; set; }

    public string? modified_by { get; set; }

    public DateTime? modified_dt { get; set; }

    public bool? isactive { get; set; }
}
