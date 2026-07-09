using GaiaWeb.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace GaiaWeb.Pages;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;

    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (!string.IsNullOrEmpty(role))
        {
            if (role == "Admin")
            {
                return RedirectToPage("/Admin/Index");
            }
            else if (role == "Supplier")
            {
                return RedirectToPage("/Supplier/Index");
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userService.LoginAsync(Input.Email, Input.Password);

        if (user == null)
        {
            ErrorMessage = "Email hoặc mật khẩu không chính xác hoặc tài khoản đã bị khóa.";
            return Page();
        }

        // Save session
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("UserName", user.FullName);
        HttpContext.Session.SetString("UserRole", user.Role);

        if (user.Role == "Supplier")
        {
            if (user.Supplier != null)
            {
                HttpContext.Session.SetInt32("SupplierId", user.Supplier.SupplierId);
            }
            return RedirectToPage("/Supplier/Index");
        }
        else if (user.Role == "Admin")
        {
            return RedirectToPage("/Admin/Index");
        }

        ErrorMessage = "Tài khoản không có vai trò hợp lệ trong hệ thống.";
        return Page();
    }
}
