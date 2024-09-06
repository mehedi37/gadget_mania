using gadget_mania.Areas.Identity.Data;
using gadget_mania.Data;
using gadget_mania.Models;
using gadget_mania.Services;
using gadget_mania.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace gadget_mania.Controllers
{
    [Authorize]
    public class SellController : Controller
    {
        private readonly ILogger<SellController> _logger;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ProductServices _productServices;
        private readonly CustomerServices _customerServices;

        public SellController(ILogger<SellController> logger, AppDbContext context, UserManager<AppUser> userManager, ProductServices productServices, CustomerServices customerServices)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _productServices = productServices;
            _customerServices = customerServices;
        }

        public async Task<IActionResult> MyGadgets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(); // or handle the null case appropriately
            }

            var itemsForSale = await _productServices.ItemsForSaleByUserIdAsync(userId);
            var customers = await _customerServices.CustomersBySellerIdAsync(userId);

            var model = new SellerViewModel
            {
                ItemsForSale = itemsForSale,
                Customers = customers
            };

            return View(model);
        }

        public IActionResult AddGadgets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return View(new Products
            {
                ProductId = new int(),
                ProductName = string.Empty,
                ProductDescription = string.Empty,
                ProductImage = string.Empty,
                ProductPrice = 0.0M,
                UserId = userId,
                Stock = 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddGadgets(Products product)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                product.UserId = userId ?? throw new InvalidOperationException("User ID cannot be null");
                await _productServices.AddItemAsync(product);
                return RedirectToAction("MyGadgets");
            }
            return View(product);
        }

        public async Task<IActionResult> EditGadgets(int id)
        {
            var product = await _productServices.ItemByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditGadgets(Products product)
        {
            if (ModelState.IsValid)
            {
                await _productServices.UpdateItemAsync(product);
                return RedirectToAction("MyGadgets");
            }
            return View(product);
        }

        public async Task<IActionResult> DeleteItem(int id)
        {
            var product = await _productServices.ItemByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            await _productServices.DeleteItemAsync(id);
            return RedirectToAction("MyGadgets");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
