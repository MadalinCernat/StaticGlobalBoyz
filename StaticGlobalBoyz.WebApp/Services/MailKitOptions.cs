using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp.Services
{
    public class MailKitOptions
    {
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string Host { get; set; }
    }
}
