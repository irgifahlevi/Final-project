using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaDanaService.Dtos
{
    public class UserBelanceDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Saldo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}