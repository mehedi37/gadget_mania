using gadget_mania.Models;

namespace gadget_mania.ViewModels
{
    public class ProductDetailsViewModel
    {
        public required Products Product { get; set; }
        public required List<Products> OtherProducts { get; set; }
    }
}
