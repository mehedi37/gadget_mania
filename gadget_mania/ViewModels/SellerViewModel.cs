using gadget_mania.Models;

namespace gadget_mania.ViewModels
{
    public class SellerViewModel
    {
        public List<Products> ItemsForSale { get; set; } = new List<Products>();
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }
}
