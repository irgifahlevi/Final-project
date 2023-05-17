using BukaToko.DTOS;
using BukaToko.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization;

namespace BukaToko.Data
{
    public class AccountRepo : IAccountRepo
    {
        private readonly BukaTokoDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountRepo(BukaTokoDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public UserToken Login(LoginUserDto user)
        {
            // linq
            var usr = _context.Users
                .Where(o => o.Username == user.Username).FirstOrDefault();
            if (usr != null)
            {
                if (BC.Verify(user.Password, usr.Password))
                {
                    // login sukses
                    // ambil role                      
                    // joins
                    var roles = from ur in _context.UserRoles
                                join r in _context.Roles
                                on ur.RoleId equals r.Id
                                where ur.UserId == usr.Id
                                select r.Name;
                    
                        var roleClaims = new Dictionary<string, object>();
                    
                    foreach (var role in roles)
                    {
                        //banned user
                        if (role == "BannedUser")
                        {
                            return new UserToken { Message = "Login is Banned by admin" };
                        }
                        roleClaims.Add(ClaimTypes.Role, "" + role);
                    }
                    
                    var secret = _configuration.GetValue<string>("AppSettings:Secret");
                    var secretBytes = Encoding.ASCII.GetBytes(secret);
                    // token
                    var expired = DateTime.Now.AddDays(2); // 2 hari
                    var tokenHandler = new JwtSecurityTokenHandler();
                    // data
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        // payload
                        Subject = new System.Security.Claims.ClaimsIdentity(
                                new Claim[]
                                {
                                    new Claim(ClaimTypes.Name, user.Username)
                                }),
                        Claims = roleClaims, // claims - roles
                        Expires = expired,
                        SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(secretBytes),
                                SecurityAlgorithms.HmacSha256Signature
                            )
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var userToken = new UserToken
                    {
                        Token = tokenHandler.WriteToken(token), // token as string
                        ExpiredAt = expired.ToString(),
                        Message = ""
                    };
                    return userToken;
                }
            }
            return new UserToken { Message = "Invalid username or password" };
        }

        public string Register(User user)
        {
            // transaction
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {

                    // tambah user
                    var u = new User();
                    u.Username = user.Username;
                    u.Password = BC.HashPassword(user.Password);

                    // cek apakah user sudah memiliki wallet
                    var wallet = _context.Wallets.FirstOrDefault(w => w.Username == user.Username);
                    if (wallet != null)
                    {
                        // set user wallet id dengan id wallet yang sudah ada
                        u.WalletId = wallet.Id;
                    }
                    else
                    {
                        // buat wallet baru
                        var newWallet = new Wallet();
                        newWallet.Username = user.Username;
                        newWallet.Cash = 0;
                        _context.Wallets.Add(newWallet);

                        // simpan perubahan ke database terlebih dahulu
                        _context.SaveChanges();

                        // set user wallet id dengan id wallet yang baru
                        u.WalletId = newWallet.Id;
                    }
                    _context.Users.Add(u);
                    // ambil role member
                    var role = _context.Roles.Where(o => o.Name == "User").FirstOrDefault();
                    if (role == null)
                    {
                        throw new Exception("role is null");
                    }
                    // assign role ke user
                    var ur = new UserRole();
                    ur.User = u;
                    ur.Role = role;
                    _context.UserRoles.Add(ur);
                    // simpan dan commit
                    _context.SaveChanges();
                    trans.Commit();
                    return "Register sukses";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return "Register gagal";
                }
            }
        }

        public string RegisterManager(User user)
        {
            // transaction
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    // tambah user
                    var u = new User();
                    u.Username = user.Username;
                    u.Password = BC.HashPassword(user.Password);

                    // cek apakah user sudah memiliki wallet
                    var wallet = _context.Wallets.FirstOrDefault(w => w.Username == user.Username);
                    if (wallet != null)
                    {
                        // set user wallet id dengan id wallet yang sudah ada
                        u.WalletId = wallet.Id;
                    }
                    else
                    {
                        // buat wallet baru
                        var newWallet = new Wallet();
                        newWallet.Username = user.Username;
                        newWallet.Cash = 0;
                        _context.Wallets.Add(newWallet);

                        // simpan perubahan ke database terlebih dahulu
                        _context.SaveChanges();

                        // set user wallet id dengan id wallet yang baru
                        u.WalletId = newWallet.Id;
                    }
                    _context.Users.Add(u);
                    // ambil role member
                    var role = _context.Roles.Where(o => o.Name == "Manager").FirstOrDefault();
                    if (role == null)
                    {
                        throw new Exception("role is null");
                    }
                    // assign role ke user
                    var ur = new UserRole();
                    ur.User = u;
                    ur.Role = role;
                    _context.UserRoles.Add(ur);
                    // simpan dan commit
                    _context.SaveChanges();
                    trans.Commit();
                    return "Register sukses";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return "Register gagal";
                }
            }
        }
        public string RegisterAdmin(User user)
        {
            // transaction
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    // tambah user
                    var u = new User();
                    u.Username = user.Username;
                    u.Password = BC.HashPassword(user.Password);

                    // cek apakah user sudah memiliki wallet
                    var wallet = _context.Wallets.FirstOrDefault(w => w.Username == user.Username);
                    if (wallet != null)
                    {
                        // set user wallet id dengan id wallet yang sudah ada
                        u.WalletId = wallet.Id;
                    }
                    else
                    {
                        // buat wallet baru
                        var newWallet = new Wallet();
                        newWallet.Username = user.Username;
                        newWallet.Cash = 0;
                        _context.Wallets.Add(newWallet);

                        // simpan perubahan ke database terlebih dahulu
                        _context.SaveChanges();

                        // set user wallet id dengan id wallet yang baru
                        u.WalletId = newWallet.Id;
                    }
                    _context.Users.Add(u);
                    // ambil role member
                    var role = _context.Roles.Where(o => o.Name == "Admin").FirstOrDefault();
                    if (role == null)
                    {
                        throw new Exception("role is null");
                    }
                    // assign role ke user
                    var ur = new UserRole();
                    ur.User = u;
                    ur.Role = role;
                    _context.UserRoles.Add(ur);
                    // simpan dan commit
                    _context.SaveChanges();
                    trans.Commit();
                    return "Register sukses";
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return "Register gagal";
                }
            }
        }
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
        [Authorize(Roles = "Admin")]
        public string Banned(BannedUserDto bannedUser)
        {
            // get username
            var user = _context.Users.FirstOrDefault(o => o.Username == bannedUser.Username);

            if (user == null)
            {
                // jika user tidak ditemukan
                throw new ArgumentException($"User with username: {bannedUser.Username} not found or is banned.");
            }

            var ur = new UserRole();
            ur.User = user;
            var role = _context.Roles.Where(o => o.Name == "BannedUser").FirstOrDefault();
            ur.Role = role;
            _context.UserRoles.Add(ur);
            _context.SaveChanges();
            return "Banned User sukses";
        }
    }
}