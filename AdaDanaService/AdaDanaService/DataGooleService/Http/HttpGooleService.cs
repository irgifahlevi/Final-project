using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdaDanaService.Data;
using AdaDanaService.Dtos;
using AdaDanaService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace AdaDanaService.DataGooleService.Http
{
    public class HttpGooleService : IHttpGooleService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AdaDanaContext _context;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleService _userRoleService;
        private readonly IWalletService _walletService;

        public HttpGooleService(HttpClient httpClient, IConfiguration configuration,
            AdaDanaContext context, IUserService userService, IRoleService roleService,
            IUserRoleService userRoleService, IWalletService walletService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _context = context;
            _userService = userService;
            _roleService = roleService;
            _userRoleService = userRoleService;
            _walletService = walletService;
        }
        public async Task<UserToken> SendLoginByGoleId(GooleIdDto gooleIdDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(gooleIdDto),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_configuration["GoleService"]}", httpContent);
            var content = await response.Content.ReadAsStringAsync();
            var allContent = JsonSerializer.Deserialize<ResponseGooleIdDto>(content);
            if (response.IsSuccessStatusCode && allContent.Username != null)
            {
                Console.WriteLine("--> Sync POST to GoleService success !");
                var userDb = await _userService.FindUsernameDb(allContent.Username);
                // Jika username tidak ditemukan
                if (userDb == null)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // ambil role user
                            var role = await _roleService.GetRoleUser();

                            var user = new User
                            {
                                Username = allContent.Username,
                                Password = BC.HashPassword(gooleIdDto.Password),
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                Deletes = false
                            };

                            if (role != null)
                            {
                                // assign role ke user
                                var ur = new UserRole();
                                ur.User = user;
                                ur.Role = role;

                                // insert role ke dalam user role
                                await _userRoleService.AddRoleUser(ur);
                            }

                            // insert user ke dalam tabel user
                            await _userService.AddUser(user);
                            await _context.SaveChangesAsync();

                            // buat wllet untuk user
                            var wallet = new Wallet
                            {
                                UserId = user.Id,
                                Saldo = 0,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };

                            // insert user ke dalam tabel wallet
                            await _walletService.AddWallet(wallet);
                            await _context.SaveChangesAsync();

                            transaction.Commit();

                        }
                        catch (Exception ex)
                        {
                            // Jika terjadi kesalahan, rollback transaksi
                            transaction.Rollback();
                            Console.WriteLine($"Error: {ex.Message}");
                            throw new Exception("Failed to saved user in database");
                        }
                    }
                }
                // Jika username ada 
                var usr = await _userService.FindUserByUsername(allContent.Username);

                var roles = await _context.UserRoles
                            .Where(ur => ur.UserId == usr.Id)
                            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                            .ToListAsync();

                var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString()),
                            new Claim(ClaimTypes.Name, usr.Username)
                        };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var secret = _configuration.GetValue<string>("AppSettings:Secret");
                var secretBytes = Encoding.ASCII.GetBytes(secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddDays(2),
                    SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(secretBytes),
                SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return new UserToken
                {
                    Token = tokenHandler.WriteToken(token),
                    ExpiredAt = tokenDescriptor.Expires?.ToString(),
                    Message = "Loggin success"
                };
            }
            // Jika reponse goole id Unauthorized
            else
            {
                Console.WriteLine("--> Sync POST to GoleService failed");
                return new UserToken { Message = $"{allContent.Message}" };
            }
        }
    }
}