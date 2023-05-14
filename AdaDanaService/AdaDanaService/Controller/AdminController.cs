using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaDanaService.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaDanaService.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AdminController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        // Banned local user
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> BannedUser(string username)
        {
            try
            {
                await _accountService.Banned(username);
                return Ok("Banned user success!");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cannot banned user!{ex.Message}");
            }
        }


        // Unbanned local user
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult> UnbannedUser(string username)
        {
            try
            {
                await _accountService.Unbanned(username);
                return Ok("Unbanned user success!");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Cannot unbanned user!{ex.Message}");
            }
        }
    }
}