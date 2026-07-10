using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Admin.Suppliers;

public class EditModel : PageModel
{
    private readonly ISupplierService _supplierService;
    public EditModel(ISupplierService supplierService) => _supplierService = supplierService;

    [BindProperty] public int SupplierId { get; set; }
    [BindProperty] public string WarehouseName { get; set; } = string.Empty;
    [BindProperty] public string Address { get; set; } = string.Empty;
    [BindProperty] public decimal Latitude { get; set; }
    [BindProperty] public decimal Longitude { get; set; }
    [BindProperty] public string FullName { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int supplierId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var supplier = await _supplierService.GetSupplierByIdAsync(supplierId);
        if (supplier == null) return NotFound();

        SupplierId = supplier.SupplierId;
        WarehouseName = supplier.WarehouseName;
        Address = supplier.Address;
        Latitude = supplier.Latitude;
        Longitude = supplier.Longitude;
        FullName = supplier.User?.FullName ?? string.Empty;
        Phone = supplier.User?.Phone ?? string.Empty;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _supplierService.UpdateSupplierAsync(
            SupplierId, WarehouseName, Address, Latitude, Longitude, FullName, Phone);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        return RedirectToPage("/Admin/Suppliers/Index", new { msg = message, ok = true });
    }
}
