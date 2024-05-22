using FiorelloMVC.Models;
using FiorelloMVC.ViewModels.Categories;

namespace FiorelloMVC.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<IEnumerable<CategoryProductVM>> GetAllWithProductAsync();
        Task<Category>GetByIdAsync(int id);
        Task<bool>ExistAsync(string name);
        Task  CreateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<bool> ExistExceptByIdAsync(int id,string name);
        Task<IEnumerable<CategoryArchiveVM>> GetAllArchiveAsync();
    }
}
