using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaDanaService.Dtos
{
    public class ResponseGooleIdDto
    {
        public string? Token { get; set; }
        public string? ExpiredAt { get; set; }
        public string Message { get; set; }
        public string? Username { get; set; }
    }
}