using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ExtendedModels
{
    public class ApiCommonResponse<T>
    {
        public bool allowStatus { get; set; } = false;
        public bool showMsg { get; set; }
        public string msg { get; set; }
        public T? contentData { get; set; }
        public bool isContentEncryted { get; set; }
        public string encryptedContentData { get; set; }

    }
    public class Registration
    {
        public user_details userdetails { get; set; }
        public string device_name { get; set; }
    }
}
