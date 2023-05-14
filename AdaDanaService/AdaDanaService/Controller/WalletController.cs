using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdaDanaService.Data;
using AdaDanaService.Dtos;
using Microsoft.AspNetCore.Authorization;
using AdaDanaService.Models;
using System.Security.Claims;
using AutoMapper;
using AdaDanaService.AsyncDataService;

namespace AdaDanaService.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly AdaDanaContext _context;
        private readonly IWalletService _walletService;
        private readonly IMapper _mapper;
        private readonly IMessageClient _messageClient;

        public WalletController(AdaDanaContext context, IWalletService walletService, IMapper mapper, IMessageClient messageClient)
        {
            _context = context;
            _mapper = mapper;
            _walletService = walletService;
            _messageClient = messageClient;
        }

        [Authorize(Roles = "User")]
        [HttpPost("topup")]
        public async Task<IActionResult> TopUp(TopUpDto topUpDto)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(userIdClaim.Value);

                await _walletService.TopUp(userId, topUpDto.Saldo);

                var topupWalletPublishDto = _mapper.Map<TopupWalletPublishDto>(topUpDto);
                topupWalletPublishDto.Event = "TopupWallet_NewPublished";
                _messageClient.PublishTopupWallet(topupWalletPublishDto);

                return Ok("Saldo berhasil ditambahkan");
            }
            catch (Exception ex)
            {
                return BadRequest($"Terjadi kesalahan: {ex.Message}");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost("cashout")]
        public async Task<IActionResult> CashOut(CashOutDto cashOutDto)
        {
            try
            {
                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                var userId = int.Parse(userIdClaim.Value);

                await _walletService.CashOut(userId, cashOutDto.Saldo);
                return Ok("Saldo berhasil ditarik");
            }
            catch (Exception ex)
            {
                return BadRequest($"Terjadi kesalahan: {ex.Message}");
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var balance = await _walletService.GetBalance(userId);
                return Ok(new { Saldo = balance });
            }
            catch (Exception ex)
            {
                return BadRequest($"Terjadi kesalahan: {ex.Message}");
            }
        }
    }
}