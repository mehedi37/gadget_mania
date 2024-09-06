using gadget_mania.Models;

namespace gadget_mania.ViewModels
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartDetails> CartDetails { get; set; } = new List<CartDetails>();
    }
}
