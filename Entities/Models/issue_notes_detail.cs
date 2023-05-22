using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class issue_notes_detail
{
    public long trans_id { get; set; }

    public long issue_id_ref { get; set; }

    public long user_id_ref { get; set; }

    public string? note { get; set; }

    public string? filename { get; set; }

    public string? filedata { get; set; }

    public string created_by { get; set; } 

    public DateTime created_dt { get; set; }

    public string? modified_by { get; set; }

    public DateTime? modified_dt { get; set; }

    public bool? isactive { get; set; }
}
