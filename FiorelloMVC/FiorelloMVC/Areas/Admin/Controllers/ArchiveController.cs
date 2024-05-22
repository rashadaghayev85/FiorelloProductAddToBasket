using FiorelloMVC.Data;
using FiorelloMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArchiveController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly AppDBContext _context;
        public ArchiveController(ICategoryService categoryService,AppDBContext context)
        {
            _categoryService = categoryService;
            _context = context; 
        }
        public async Task<IActionResult> CategoryArchive()
        {
            return View(await _categoryService.GetAllArchiveAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SetFromArchive(int? id)
        {
            if (id is null) return BadRequest();
            var category = await _categoryService.GetByIdAsync((int)id);
            if (category is null) return NotFound();
            category.SoftDeleted = !category.SoftDeleted;
            // category.SoftDeleted = true;

            await _context.SaveChangesAsync();

            return Ok(category);
        }
    }
}
