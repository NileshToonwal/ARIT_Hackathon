using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ExtendedModels
{
    public class IssueListModel
    {
        public long  IssueId { get; set; }
        public long UserId { get; set; }

        public string Status { get; set; }
        public string IssueCretedBy { get; set; }

        //public string MyProperty { get; set; }


    }
}
