using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using System;
using System.Threading.Tasks;

namespace GaiaWeb.Pages.Story
{
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;

        public IndexModel(IProductService productService)
        {
            _productService = productService;
        }

        public Product? ProductInfo { get; set; }

        public async Task<IActionResult> OnGetAsync(string token)
        {
            if (string.IsNullOrEmpty(token) || !Guid.TryParse(token, out Guid qrToken))
            {
                return NotFound(); // Token is missing or invalid format
            }

            ProductInfo = await _productService.GetProductStoryAsync(qrToken);

            if (ProductInfo == null)
            {
                return NotFound(); // Product not found
            }

            return Page();
        }
    }
}
