using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlarumLite.core.Models
{
    public class LoginData
    {
        public string token { get; set; }
        public int userId { get; set; }
        public string statusCode { get; set; }
    }
}
