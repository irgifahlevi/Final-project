using BukaToko.Data;
using BukaToko.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BukaToko.Models;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace BukaToko.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    { 
        private readonly IProductRepo _product;
        private readonly IMapper _mapper;
        //private readonly IMessageBusClient _messageBusClient;
        public ProductController(IProductRepo productRepo, IMapper mapper)
        {
            _product = productRepo;
            _mapper = mapper;
            //_messageBusClient = messageBusClient;
        }
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto createProductDto)
        {

            var product = _mapper.Map<Product>(createProductDto);
            _product.Create(product);
            _product.SaveChanges();

            var readProductDto = _mapper.Map<ReadProductDto>(product);

            return Ok(product);
        }
        [Authorize(Roles = "Manager,User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _product.GetAll();
            var productReadDtoList = _mapper.Map<IEnumerable<ReadProductDto>>(products);
            return Ok(productReadDtoList);
        }
        [Authorize(Roles = "Manager,User")]
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _product.GetById(id);
            var readProductDto = _mapper.Map<ReadProductDto>(product);
            return Ok(readProductDto);
        }
        [Authorize(Roles = "Manager,User")]
        [HttpGet("name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var product = await _product.GetByName(name);
            var readProductDto = _mapper.Map<ReadProductDto>(product);
            return Ok(readProductDto);
        }
        [Authorize(Roles = "Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                var product = _mapper.Map<Product>(updateProductDto);
                product.Id = id;
                await _product.Update(id, product);
                _product.SaveChanges();
                var readProductDto = _mapper.Map<ReadProductDto>(product);
                return Ok(readProductDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Manager")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _product.Delete(id);
                _product.SaveChanges();
                return Ok(new { message = $"Product with Id ({id}) has been deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
