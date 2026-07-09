using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Supplier;

public class IndexModel : PageModel
{
    public string UserName { get; set; } = string.Empty;
    public int SupplierId { get; set; }

    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
        {
            return RedirectToPage("/Login");
        }

        UserName = HttpContext.Session.GetString("UserName") ?? "Supplier";
        SupplierId = HttpContext.Session.GetInt32("SupplierId") ?? 0;

        return Page();
    }
}
