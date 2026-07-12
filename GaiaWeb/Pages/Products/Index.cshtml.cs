using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public List<Product> Products { get; set; } = new();

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public async Task OnGetAsync()
        {
            // Lấy tất cả sản phẩm để hiển thị trong danh sách
            Products = await _productService.GetAllProductsAsync();
        }
    }
}
