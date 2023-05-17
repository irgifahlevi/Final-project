using AutoMapper;
using BukaToko.Data;
using BukaToko.DTOS;
using BukaToko.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BukaToko.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepo _walletRepo;
        private readonly IMapper _mapper;

        public WalletController(IWalletRepo walletRepo, IMapper mapper)
        {
            _walletRepo = walletRepo;
            _mapper = mapper;
        }
        [Authorize(Roles = "Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
        {
            var wallets = await _walletRepo.GetAllWallets();
            var walletReadDtoList = _mapper.Map<IEnumerable<ReadWalletDto>>(wallets);
            return Ok(walletReadDtoList);
        }
        [Authorize(Roles = "User")]
        [HttpPost("{username}/{amount}")]
        public async Task<IActionResult> TopUpWallet(string username,int amount)
        {
            var wallet = await _walletRepo.TopUp(username,amount);
            var readWallet = _mapper.Map<ReadWalletDto>(wallet);
            _walletRepo.SaveChanges();
            return Ok(readWallet);
        }
    }
}
