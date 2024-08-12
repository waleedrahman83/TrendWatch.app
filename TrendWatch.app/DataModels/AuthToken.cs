using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendWatch.App.DataModels
{
    internal class AuthToken
    {
        public string message { get; set; }
        public bool isAuthenticated { get; set; }
        public string mobileNumber { get; set; }
        public string email { get; set; }
        public List<string> roles { get; set; }
        public string token { get; set; }
        public DateTime expiresOn { get; set; }
        public Guid id { get; set; }
        public string name { get; set; }

        public string FullName { get; set; }
    }
}
