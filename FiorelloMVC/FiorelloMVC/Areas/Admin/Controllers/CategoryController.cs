﻿using FiorelloMVC.Data;
using FiorelloMVC.Models;
using FiorelloMVC.Services.Interfaces;
using FiorelloMVC.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiorelloMVC.Areas.Admin.Controllers
{
        [Area("admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly AppDBContext _context;
        public CategoryController(ICategoryService categoryService,
                                   AppDBContext context)
        {
            _categoryService = categoryService;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View( await _categoryService.GetAllWithProductAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(CategoryCreateVM category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool existCategory = await _categoryService.ExistAsync(category.Name);
            if (existCategory)
            {
                ModelState.AddModelError("Name","This category already exist");
                return View();
            }
            await _categoryService.CreateAsync(new Category { Name = category.Name });
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var category = await _categoryService.GetByIdAsync((int)id);
            if (category is null) return NotFound();
            await _categoryService.DeleteAsync(category);
            if (category.SoftDeleted)   
                return RedirectToAction("CategoryArchive","Archive");
               
            
            return RedirectToAction(nameof(Index));
                


        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            Category category = await _context.Categories.Include(m=>m.Products).ThenInclude(m=>m.ProductImages).FirstOrDefaultAsync(m=>m.Id==id);
             
           
            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();
            var category = await _categoryService.GetByIdAsync((int)id);
            if (category is null) return NotFound();
            return View(new CategoryEditVM { Name = category.Name });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, CategoryEditVM request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (id is null) return BadRequest();

            if (await _categoryService.ExistExceptByIdAsync((int)id, request.Name)) ;
            {
                ModelState.AddModelError("Name", "This category already exist");
                return View();
            }
            var category = await _categoryService.GetByIdAsync((int)id);
            
            if (category is null) return NotFound();
            
            if (category.Name == request.Name)
            {
                return RedirectToAction(nameof(Index));
            }

            
            
            category.Name = request.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
         public async Task<IActionResult> SetToArchive(int? id)
        {
            if (id is null) return BadRequest();
            var category = await _categoryService.GetByIdAsync((int)id);
            if (category is null) return NotFound();
            category.SoftDeleted=!category.SoftDeleted;
           // category.SoftDeleted = true;
            await _context.SaveChangesAsync();
           
            return Ok(category);
        }
    }
}
