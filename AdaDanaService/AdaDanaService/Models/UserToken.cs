using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaDanaService.Models
{
    public class UserToken
    {
        public string Token { get; set; }
        public string ExpiredAt { get; set; }
        public string Message { get; set; }
    }
}