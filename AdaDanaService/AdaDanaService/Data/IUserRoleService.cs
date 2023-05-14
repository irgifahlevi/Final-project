using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaDanaService.Models;

namespace AdaDanaService.Data
{
    public interface IUserRoleService
    {
        Task AddRoleUser(UserRole userRole);
    }
}