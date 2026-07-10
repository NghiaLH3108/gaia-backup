using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SupplierEntity = GaiaWeb.DAL.Models.Supplier;

namespace GaiaWeb.Pages.Admin.Suppliers;

public class IndexModel : PageModel
{
    private readonly ISupplierService _supplierService;
    public IndexModel(ISupplierService supplierService) => _supplierService = supplierService;

    public List<SupplierEntity> Suppliers { get; set; } = new();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(string? msg, bool? ok)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        Suppliers = await _supplierService.GetAllSuppliersAsync();
        Message = msg;
        IsSuccess = ok ?? true;
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int supplierId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _supplierService.SoftDeleteSupplierAsync(supplierId);
        return RedirectToPage(new { msg = message, ok = success });
    }

    public async Task<IActionResult> OnPostRestoreAsync(int supplierId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _supplierService.RestoreSupplierAsync(supplierId);
        return RedirectToPage(new { msg = message, ok = success });
    }
}
