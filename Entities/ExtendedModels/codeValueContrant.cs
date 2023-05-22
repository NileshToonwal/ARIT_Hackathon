using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ExtendedModels
{
    public class CodeValueContrant
    {
        public static string PanRegex= @"^([A-Z]{5}[0-9]{4}[A-Z]{1})$";

        public static   string EmailRegex = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
    }
}
