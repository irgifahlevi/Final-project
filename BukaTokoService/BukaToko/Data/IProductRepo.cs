using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BukaToko.Models;

namespace BukaToko.Data
{
    public interface IProductRepo
    {
        Task<IEnumerable<Product>> GetAll();
        Task<Product> GetById(int id);
        Task<Product> GetByName(string name);
        Task Create(Product product);
        Task Update(int id, Product product);
        Task Delete(int id);
        bool SaveChanges();
    }
}
