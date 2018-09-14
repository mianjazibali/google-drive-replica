using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace googledrive.Models
{
    public class LoginDTO
    {
        public int Id { set; get; }
        public string Login { set; get; }
        public string IpAddress { set; get; }
        public string LoggedOn { set; get; }
    }
}