using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;


        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public IQueryable<Product> Products => _context.Products;

        public void SaveProduct(Product product)
        {
            if (product.ProductId == 0)
            {
                _context.Products.Add(product);
            }
            else
            {
                Product dbEntity = _context.Products
                    .FirstOrDefault(p => p.ProductId == product.ProductId);

                if (dbEntity != null)
                {
                    dbEntity.Name = product.Name;
                    dbEntity.Description = product.Description;
                    dbEntity.Price = product.Price;
                    dbEntity.Category = product.Category;
                }
            }
            _context.SaveChanges();
        }

        public Product DeleteProduct(int productId)
        {
            Product dbEntity = _context.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (dbEntity != null)
            {
                _context.Products.Remove(dbEntity);
                _context.SaveChanges();
            }

            return dbEntity;
        }
    }
}
