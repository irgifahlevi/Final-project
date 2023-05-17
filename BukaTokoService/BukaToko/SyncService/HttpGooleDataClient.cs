using BukaToko.DTOS;
using System.Text.Json;
using System.Text;
using BukaToko.Models;
using BukaToko.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace BukaToko.SyncService
{
    public class HttpGooleDataClient : IGooleDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IAccountRepo _accountRepo;
        private readonly BukaTokoDbContext _context;
        private readonly IMapper _mapper;

        public HttpGooleDataClient(HttpClient httpClient, IConfiguration configuration, IAccountRepo accountRepo, BukaTokoDbContext context,IMapper mapper)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _accountRepo = accountRepo;
            _context = context;
            _mapper = mapper;

        }
        public async Task<UserToken> SendUserToGoole(LoginUserDto user)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync($"{_configuration["GooleService"]}", httpContent);
            // jika digoole return succes
            if (response.IsSuccessStatusCode)
            {
                var u = await _context.Users.FirstOrDefaultAsync(p => p.Username == user.Username);
                if (u == null)
                {
                    //throw new Exception("silahkan registrasi dahulu di bukatoko dengan username yang sama");
                    //diregis dulu baru login
                    var userRegister = _mapper.Map<User>(user);
                    _accountRepo.Register(userRegister);
                    await _context.SaveChangesAsync();
                    return _accountRepo.Login(user);
                }
                return _accountRepo.Login(user);
            }
            else
            {
                throw new Exception("Username Password Invalid");
            }         
        }
    }
}
