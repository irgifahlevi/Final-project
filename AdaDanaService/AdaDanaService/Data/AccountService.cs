using AdaDanaService.Dtos;
using AdaDanaService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using BC = BCrypt.Net.BCrypt;

namespace AdaDanaService.Data
{
    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IUserRoleService _userRoleService;
        private readonly IConfiguration _configuration;
        private readonly AdaDanaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWalletService _walletService;

        public AccountService(IUserService userService, IRoleService roleService,
        IMapper mapper, IUserRoleService userRoleService, IConfiguration configuration,
        AdaDanaContext context, IHttpContextAccessor httpContextAccessor,
        IWalletService walletService)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _userRoleService = userRoleService;
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _walletService = walletService;
        }

        // Register admin dan manager
        public async Task<bool> Register(RegisterUserDto registerUserDto)
        {
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    var existingUser = await _userService.FindUser(registerUserDto.Username);
                    if (existingUser != null)
                    {
                        throw new Exception($"User {registerUserDto.Username} already exists");
                    }
                    // ambil role user
                    var role = await _roleService.GetRoleUser(); // Hardcode langsung Admin

                    // mapping register user dto ke user
                    var user = new User
                    {
                        Username = registerUserDto.Username,
                        Password = BC.HashPassword(registerUserDto.Password),
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

                        // insert userId ke tabel userRole
                        await _userRoleService.AddRoleUser(ur);
                    }

                    // insert user ke dalam tabel user
                    await _userService.AddUser(user);
                    await _context.SaveChangesAsync();

                    //membuat wallet dengan saldo 0
                    var wallet = new Wallet
                    {
                        UserId = user.Id,
                        Saldo = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // insert userId ke dalam tabel wallet
                    await _walletService.AddWallet(wallet);
                    await _context.SaveChangesAsync();

                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Jika terjadi kesalahan, rollback transaksi
                    trans.Rollback();
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return false;
        }

        // Login admin & manager
        public async Task<UserToken> Login(LoginDto login)
        {
            var usr = await _userService.FindUser(login.Username);
            if (usr != null)
            {
                if (BC.Verify(login.Password, usr.Password))
                {
                    if (usr.Deletes == true)
                    {
                        return new UserToken { Message = "Your account is disabled, please contact adadana support" };
                    }
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
            }
            return new UserToken { Message = "Invalid username or password" };
        }

        public async Task UpdatePasswordUser(UpdatePassword updatePassword)
        {
            var user = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (user == null)
                throw new ArgumentException("User not found");

            // ambil user
            var usr = await _userService.GetUser(user);
            usr.Password = BC.HashPassword(updatePassword.NewPassword);
            usr.UpdatedAt = DateTime.Now;
            await _userService.UpdateUser(usr);
        }

        public async Task Banned(string username)
        {
            var userBanned = await _userService.FindUserNotBanned(username);
            if (userBanned == null)
            {
                throw new Exception("User not found or not allowed to baned");
            }
            userBanned.Deletes = true;
            await _userService.UpdateUser(userBanned);
        }

        public async Task Unbanned(string username)
        {
            var userUnbanned = await _userService.FindUserBanned(username);
            if (userUnbanned == null)
            {
                throw new Exception("User not found or not banned");
            }
            userUnbanned.Deletes = false;
            await _userService.UpdateUser(userUnbanned);
        }
    }
}
