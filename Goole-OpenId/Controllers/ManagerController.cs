using Goole_OpenId.Data;
using Goole_OpenId.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Goole_OpenId.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        public ManagerController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        [Authorize(Roles = "manager")]
        [HttpGet("Users")]
        public async Task<IActionResult> ListUsers()
        {
            try
            {
                var users = await _userRepo.GetAllUser();

                if (users == null)
                {
                    return NotFound();
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                // Handle exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
