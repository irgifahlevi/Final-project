using AdaDanaService.Data;
using AdaDanaService.DataGooleService.Http;
using AdaDanaService.Dtos;
using AdaDanaService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdaDanaService.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IHttpGooleService _httpGooleService;

        public AccountController(IAccountService accountService, IHttpGooleService httpGooleService)
        {
            _accountService = accountService;
            _httpGooleService = httpGooleService;

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserDto registerUser)
        {
            var result = await _accountService.Register(registerUser);

            if (result)
            {
                return Ok("User registered successfully");
            }
            else
            {
                return BadRequest("Failed to register user");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserToken>> Login(LoginDto user)
        {
            var result = await _accountService.Login(user);
            if (result != null)
            {
                return result;
            }
            return BadRequest(new UserToken { Message = "Invalid username or password" });
        }

        [Authorize(Roles = "User")]
        [HttpPut]
        public async Task<string> ChangePassword(UpdatePassword updatePassword)
        {
            try
            {
                await _accountService.UpdatePasswordUser(updatePassword);
                return "Password updated successfully";
            }
            catch (ArgumentException ex)
            {
                return $"Error: {ex.Message}";
            }
            catch (Exception)
            {
                return "Error updating password";
            }
        }


        [HttpPost]
        public async Task<ActionResult<UserToken>> LoginByGole(GooleIdDto gooleIdDto)
        {
            try
            {
                var result = await _httpGooleService.SendLoginByGoleId(gooleIdDto);
                if (result.Token != null)
                    return result;
                else
                    throw new Exception($"{result.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
                return BadRequest(new UserToken { Message = ex.Message });
            }
        }



    }
}
