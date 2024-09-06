using gadget_mania.Areas.Identity.Data;
using gadget_mania.Data;
using gadget_mania.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace gadget_mania.Services
{
    public class CartServices
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartServices(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<CartDetails>> CartDetailsAsync(AppUser user)
        {
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Products)
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsPurchased);

            if (cart == null || cart.CartDetails == null)
            {
                return new List<CartDetails>();
            }

            return cart.CartDetails.ToList();
        }

        public async Task<Cart?> CartByUserIdAsync(string userId)
        {
            var cart = await _context.Cart
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Products)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsPurchased);

            if (cart == null || cart.CartDetails == null)
            {
                return null;
            }
            return cart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await CartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = new int(),
                    UserId = userId,
                    IsPurchased = false
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var CartDetails = await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.CartId == cart.CartId && cd.ProductId == productId);

            if (CartDetails == null)
            {
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }

                CartDetails = new CartDetails
                {
                    CartDetailsId = new int(),
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.ProductPrice
                };
                _context.CartDetails.Add(CartDetails);
            }
            else
            {
                CartDetails.Quantity += quantity;
                _context.CartDetails.Update(CartDetails);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(int cartDetailsId, int quantity)
        {
            var CartDetails = await _context.CartDetails.FindAsync(cartDetailsId);
            if (CartDetails != null)
            {
                CartDetails.Quantity = quantity;
                _context.CartDetails.Update(CartDetails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int cartDetailsId)
        {
            var CartDetails = await _context.CartDetails.FindAsync(cartDetailsId);
            if (CartDetails != null)
            {
                _context.CartDetails.Remove(CartDetails);
                await _context.SaveChangesAsync();

                var cart = await _context.Cart
                    .Include(c => c.CartDetails)
                    .FirstOrDefaultAsync(c => c.CartId == CartDetails.CartId);

                if (cart != null && !cart.CartDetails.Any())
                {
                    _context.Cart.Remove(cart);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task<int> CartDetailsCountAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            return cart?.CartDetails.Count ?? 0;
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            if (cart != null)
            {
                _context.CartDetails.RemoveRange(cart.CartDetails);
                _context.Cart.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }
        public async Task PurchaseCartAsync(string userId)
        {
            var cart = await CartByUserIdAsync(userId);
            if (cart != null)
            {
                cart.IsPurchased = true;
                _context.Cart.Update(cart);
                await _context.SaveChangesAsync();
            }
        }
    }
}
