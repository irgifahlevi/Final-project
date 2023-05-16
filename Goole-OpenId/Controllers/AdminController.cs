using Azure.Identity;
using Goole_OpenId.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goole_OpenId.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "admin")]
        [HttpPut("Banned")]
        public async Task<string> BannedUser(string username)
        {
            try
            {
                var result = await _userService.Banned(username);
                if (result == true)
                {
                    return "Banned Success!";
                }
                else
                {
                    return "Unbanned Success!";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot banned/unbanned user!");
            }
        }
    }
}
