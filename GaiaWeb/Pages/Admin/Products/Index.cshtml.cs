using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Admin.Products;

public class IndexModel : PageModel
{
    private readonly IProductService _productService;
    public IndexModel(IProductService productService) => _productService = productService;

    public List<Product> Products { get; set; } = new();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(string? msg, bool? ok)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        Message = msg;
        IsSuccess = ok ?? true;
        Products = await _productService.GetAllProductsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int productId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _productService.DeleteProductAsync(productId);
        return RedirectToPage(new { msg = message, ok = success });
    }
}
