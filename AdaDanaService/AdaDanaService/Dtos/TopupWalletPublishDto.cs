using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaDanaService.Dtos
{
    public class TopupWalletPublishDto
    {
        public string Username { get; set; }
        public int Saldo { get; set; }
        public string Event { get; set; } = string.Empty;
    }
}