using BukaToko.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BukaToko.DTOS;
using BukaToko.Models;
using BukaToko.DTOS;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Net.Http;
using BukaToko.ASyncService;
using BukaToko.AsyncService;


//TODO: ganti tempname sama user dari jwt nanti
//TODO: ganti input nama barang dengan nama dari id produk di AddCart()
//TODO: kelarin GetOrder()

namespace BukaToko.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IProductRepo _productRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;
        private readonly IMessageBusClient _messageBusClient;
        //private string tempName = "akun1";

        public OrderController(IOrderRepo orderRepo, IMapper mapper, IProductRepo productRepo, IHttpContextAccessor httpContextAccessor, IMessageBusClient messageBusClient)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _productRepo = productRepo;
            _httpContext = httpContextAccessor.HttpContext;
            _messageBusClient = messageBusClient;
        }

        //list order buat manager
        [Authorize(Roles ="Manager")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReadOrderDto>>> GetAllOrder()
        {

            var orders = await _orderRepo.GetAllOrder();
            var listorder = _mapper.Map<IEnumerable<ReadOrderDto>>(orders);
            return Ok(listorder);
        }
        [Authorize(Roles = "Manager,User")]
        [HttpGet("Cart")]
        public async Task<IActionResult> GetCart()
        {
            //var tempName = "akun1";
            var user = _httpContext.User.FindFirstValue(ClaimTypes.Name);
            var userId = await _orderRepo.GetUserId(user);
            if (userId == null) return NotFound("user not found");

            var listCart = await _orderRepo.GetListCartUser(userId.Value);
            if (listCart == null) return Ok(null);

            var temp = new List<ReadCartDto>();
            foreach (var item in listCart)
            {
                temp.Add(new ReadCartDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                });
            }
            return Ok(temp);
        }

        [Authorize(Roles ="User")]
        [HttpPost("{productId}/{qty}")]
        public async Task<IActionResult> AddToCart(int productId,int qty)
        {

            //var tempName = "akun1";
            var user = _httpContext.User.FindFirstValue(ClaimTypes.Name);
            var userId = await _orderRepo.GetUserId(user);
            if (userId == null) return BadRequest("user not found");

            //pake userId.Value karna return nya nullable.
            //cant convert int? -> int


            try
            {
                var product = await _productRepo.GetById(productId);
                var cart = new Cart
                {
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = qty
                };
                await _orderRepo.AddToCart(userId.Value, cart);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

        }
        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQty(int id, int qty)
        {
            var user = _httpContext.User.FindFirstValue(ClaimTypes.Name);
            var userId = await _orderRepo.GetUserId(user);
            if (userId == null) return BadRequest("user not found");

            var cart = await _orderRepo.GetCartById(userId.Value,id);
            if (cart == null) return BadRequest("Cart not found");

            await _orderRepo.UpdateQty(userId.Value,cart.Id, qty);
            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFromCart(int id)
        {
            //var tempName = "akun1";
            var user = _httpContext.User.FindFirstValue(ClaimTypes.Name);
            var userId = await _orderRepo.GetUserId(user);
            if (userId == null) return BadRequest("user not found");

            var cart = await _orderRepo.GetCartById(userId.Value, id);
            if (cart == null) return BadRequest("Cart not found");

            await _orderRepo.DeleteFromCart(userId.Value,cart.Id);
            return Ok();
        }

        [Authorize(Roles = "User")]
        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var user = _httpContext.User.FindFirstValue(ClaimTypes.Name);
                var userId = await _orderRepo.GetUserId(user);
                if (userId == null) return BadRequest("user not found");
                await _orderRepo.Checkout(userId.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize(Roles ="Manager")]
        [HttpGet("ShippedOrder")]
        public async Task<IActionResult> ShippedOrder(int OrderId)
        {
            await _orderRepo.Shipped(OrderId);
            return Ok();
        }

    }
}
