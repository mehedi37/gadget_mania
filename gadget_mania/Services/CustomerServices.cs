using gadget_mania.Data;
using gadget_mania.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace gadget_mania.Services
{
    public class CustomerServices
    {
        private readonly AppDbContext _context;

        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerViewModel>> CustomersBySellerIdAsync(string sellerId)
        {
            // Fetch the necessary data without performing the aggregation
            var cartData = await _context.Cart
                .Where(c => c.IsPurchased && c.CartDetails.Any(cd => cd.Products.UserId == sellerId))
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Products)
                .Include(c => c.AppUser)
                .ToListAsync();

            // Perform the aggregation in memory
            var customers = cartData
                .GroupBy(c => c.UserId)
                .Select(g => new CustomerViewModel
                {
                    CustomerName = g.First().AppUser.FirstName,
                    TotalSpent = g.Sum(c => c.CartDetails.Sum(cd => cd.Price * cd.Quantity))
                })
                .ToList();

            return customers;
        }
    }
}
