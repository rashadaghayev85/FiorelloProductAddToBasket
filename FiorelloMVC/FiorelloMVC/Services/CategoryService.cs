using FiorelloMVC.Data;
using FiorelloMVC.Models;
using FiorelloMVC.Services.Interfaces;
using FiorelloMVC.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;

namespace FiorelloMVC.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly AppDBContext _context;
        public CategoryService(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Category category)
        {
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(string name)
        {
            return await _context.Categories.AnyAsync(m => m.Name.Trim() == name.Trim());          
            
        }

        public async Task<bool> ExistExceptByIdAsync(int id,string name)
        {
            return await _context.Categories.AnyAsync(m=>m.Name==name&&m.Id!=id);
        }

        public async Task<IEnumerable<CategoryArchiveVM>> GetAllArchiveAsync()
        {
            IEnumerable<Category> categories = await _context.Categories.IgnoreQueryFilters()
                             .Where(m => m.SoftDeleted)
                             .OrderByDescending(m => m.Id)
                             .ToListAsync();
            return categories.Select(m => new CategoryArchiveVM
            {
                CategoryName = m.Name,
                Id = m.Id,
                CreatedDate = m.CreatedDate.ToString("dd.MM.yyyy"),
            });

        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<IEnumerable<CategoryProductVM>> GetAllWithProductAsync()
        {
            IEnumerable<Category> categories = await _context.Categories.Include(m => m.Products)
                                                                       .OrderByDescending(m => m.Id)
                                                                       .ToListAsync();
            return categories.Select(m => new CategoryProductVM
            {
                CategoryName = m.Name,
                Id = m.Id,
                CreatedDate = m.CreatedDate.ToString("dd.MM.yyyy"),
                ProductCount = m.Products.Count
            });
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.IgnoreQueryFilters().FirstOrDefaultAsync(m=>m.Id==id);  
        }
    }

     
       

        
    
}
