using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Admin;

public class IndexModel : PageModel
{
    public string UserName { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Admin")
        {
            return RedirectToPage("/Login");
        }

        UserName = HttpContext.Session.GetString("UserName") ?? "Admin";

        return Page();
    }
}
