using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleDrive.Models
{
    public class UserDTO
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Login { set; get; }
        public string Password { set; get; }
        public string Email { set; get; }
        public string Token { set; get; }
        public string TokenPassword { set; get; }
    }
}