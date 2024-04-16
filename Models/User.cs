using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models
{
    public class User
    {
        public int userId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmpassword { get; set; }
        public string bio { get; set; }
        public int reading { get; set; }
        public int contribute { get; set; }
        public IFormFile profileimg { get; set; }
        public string profileurl { get; set; }
        public roles role { get; set; }

        public enum roles
        {
            Admin,
            Customer
        }
    }
}
