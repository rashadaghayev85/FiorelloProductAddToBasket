using FiorelloMVC.Data;
using FiorelloMVC.Models;
using FiorelloMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiorelloMVC.Services
{
    public class ProductService:IProductService
    {
        private readonly AppDBContext _context;
        public ProductService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(m=>m.ProductImages).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> GetByIdWithAllDatasAsync(int id)
        {
            return await _context.Products.Where(m => m.Id == id)
                .Include(m => m.Category)
                .Include(m => m.ProductImages)
                .FirstOrDefaultAsync();
        }
    }
}
