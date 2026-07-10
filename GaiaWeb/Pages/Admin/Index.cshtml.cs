using GaiaWeb.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ISupplierService _supplierService;
    private readonly IMaterialBatchService _batchService;
    private readonly IProductService _productService;

    public IndexModel(ISupplierService supplierService, IMaterialBatchService batchService, IProductService productService)
    {
        _supplierService = supplierService;
        _batchService = batchService;
        _productService = productService;
    }

    public string UserName { get; set; } = string.Empty;
    public int TotalSuppliers { get; set; }
    public int PendingBatches { get; set; }
    public int TotalProducts { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Admin")
            return RedirectToPage("/Login");

        UserName = HttpContext.Session.GetString("UserName") ?? "Admin";

        var suppliers = await _supplierService.GetAllSuppliersAsync();
        TotalSuppliers = suppliers.Count;

        var batches = await _batchService.GetAllBatchesAsync();
        PendingBatches = batches.Count(b => b.Status == "Pending");

        var products = await _productService.GetAllProductsAsync();
        TotalProducts = products.Count;

        return Page();
    }
}
