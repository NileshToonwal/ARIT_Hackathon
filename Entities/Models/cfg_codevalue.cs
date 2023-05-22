using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class cfg_codevalue
{
    public int codevalueid { get; set; }

    public string codetype { get; set; } 

    public string codevalue { get; set; } 

    public string codevaluedescription { get; set; } 

    public short displaysequence { get; set; }

    public bool isdefault { get; set; }

    public int? parentcodevalueid { get; set; }

    public string? additionalinfo { get; set; }

    public string createdby { get; set; } 

    public DateTime createddate { get; set; }

    public string? modifiedby { get; set; }

    public DateTime? modifieddate { get; set; }

    public bool? isenable { get; set; }

    public bool? isactive { get; set; }
}
