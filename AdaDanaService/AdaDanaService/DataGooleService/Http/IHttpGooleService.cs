using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaDanaService.Dtos;
using AdaDanaService.Models;

namespace AdaDanaService.DataGooleService.Http
{
    public interface IHttpGooleService
    {
        Task<UserToken> SendLoginByGoleId(GooleIdDto gooleIdDto);
    }
}