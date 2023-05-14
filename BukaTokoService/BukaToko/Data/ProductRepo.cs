using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BukaToko.Data;
using BukaToko.Models;
namespace BukaToko.Data
{
    public class ProductRepo : IProductRepo
    {
        private readonly BukaTokoDbContext _context;

        public ProductRepo(BukaTokoDbContext context)
        {
            _context = context;
        }
        public async Task Create(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _context.Products.Add(product);
        }

        public async Task Delete(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            _context.Products.Remove(product);
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        public async Task<Product> GetByName(string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            return product;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public async Task Update(int id, Product product)
        {
            try
            {
                var existingProduct = await GetById(product.Id);

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
