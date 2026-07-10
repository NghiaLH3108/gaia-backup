using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GaiaWeb.Pages.Admin.Products;

public class CreateModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IMaterialBatchService _batchService;

    public CreateModel(IProductService productService, IMaterialBatchService batchService)
    {
        _productService = productService;
        _batchService = batchService;
    }

    [BindProperty] public int BatchId { get; set; }
    [BindProperty] public string ProductName { get; set; } = string.Empty;
    [BindProperty] public string Description { get; set; } = string.Empty;
    [BindProperty] public string CurrentStatus { get; set; } = "Available";

    public List<SelectListItem> BatchOptions { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        await LoadBatchOptions();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _productService.CreateProductAsync(BatchId, ProductName, Description, CurrentStatus);

        if (!success)
        {
            ErrorMessage = message;
            await LoadBatchOptions();
            return Page();
        }

        return RedirectToPage("/Admin/Products/Index", new { msg = message, ok = true });
    }

    private async Task LoadBatchOptions()
    {
        var batches = await _batchService.GetAllBatchesAsync();
        BatchOptions = batches
            .Where(b => b.Status == "Approved" || b.Status == "ArrivedFactory")
            .Select(b => new SelectListItem
            {
                Value = b.BatchId.ToString(),
                Text = $"{b.BatchCode} — {b.Supplier?.WarehouseName ?? ""} ({b.WeightKg} kg)"
            })
            .ToList();
    }
}
