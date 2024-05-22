using FiorelloMVC.Data;
using FiorelloMVC.Models;
using FiorelloMVC.Services.Interfaces;
using FiorelloMVC.ViewComponents;
using FiorelloMVC.ViewModels.Baskets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FiorelloMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _accessor;
        private readonly AppDBContext _context;
        public CartController(IProductService productService, IHttpContextAccessor accessor,
                                   AppDBContext context)
        {
            _productService = productService;
            _accessor = accessor;
            _context = context;

        }
        public IActionResult Index()
        {
            List<Product> pro = new();

            var basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);


            foreach (var item in basketDatas)
            {
                int id = item.Id;
                var product = _context.Products.Include(m => m.ProductImages).FirstOrDefault();
                pro.Add(product);

            }
            List<CardVM> items = pro.Select(product => new CardVM
            {
                Id = product.Id,
                Image = product.ProductImages.FirstOrDefault(m => m.IsMain).Name,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Count = basketDatas.Find(m => m.Id == product.Id).Count,
                Price = product.Price,
            }).ToList();
            TotalBasketVM model = new()
            {
                Product = items,
                Price = basketDatas.Sum(m => m.Price * m.Count),
            };
            //return await Task.FromResult(View(model));
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();
            var product = await _productService.GetByIdAsync((int)id);
            if (product is null) return NotFound();
            List<BasketVM> basketDatas;
            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                basketDatas = JsonConvert.DeserializeObject<List<BasketVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                basketDatas = new List<BasketVM>();
            }

            var existBasketData = basketDatas.FirstOrDefault(m => m.Id == id);
            if (existBasketData is not null)
            {
                _accessor.HttpContext.Response.Cookies.Delete("basket");
            }
          

            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketDatas));

            int count = basketDatas.Sum(m => m.Count);
            decimal totalPrice = basketDatas.Sum(m => m.Price * m.Count);
            return Ok(new { count, totalPrice });
        }
    }
}
