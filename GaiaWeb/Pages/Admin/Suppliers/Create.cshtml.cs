using GaiaWeb.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GaiaWeb.Pages.Admin.Suppliers;

public class CreateModel : PageModel
{
    private readonly ISupplierService _supplierService;
    public CreateModel(ISupplierService supplierService) => _supplierService = supplierService;

    [BindProperty] public string WarehouseName { get; set; } = string.Empty;
    [BindProperty] public string Address { get; set; } = string.Empty;
    [BindProperty] public decimal Latitude { get; set; }
    [BindProperty] public decimal Longitude { get; set; }
    [BindProperty] public string FullName { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _supplierService.CreateSupplierAsync(
            WarehouseName, Address, Latitude, Longitude,
            FullName, Phone, Email, Password);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        return RedirectToPage("/Admin/Suppliers/Index", new { msg = message, ok = true });
    }
}
