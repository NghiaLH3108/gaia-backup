using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Supplier.Batches;

public class IndexModel : PageModel
{
    private readonly IMaterialBatchService _batchService;

    public IndexModel(IMaterialBatchService batchService)
    {
        _batchService = batchService;
    }

    public string SupplierName  { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public List<MaterialBatch> Batches { get; set; } = new();

    public int CountPending      { get; set; }
    public int CountTransporting { get; set; }
    public int CountArrived      { get; set; }
    public int CountApproved     { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
            return RedirectToPage("/Login");

        SuccessMessage = TempData["SuccessMessage"] as string;

        var supplierId  = HttpContext.Session.GetInt32("SupplierId") ?? 0;
        SupplierName    = HttpContext.Session.GetString("UserName") ?? "";
        WarehouseName   = HttpContext.Session.GetString("WarehouseName") ?? "";

        var all = await _batchService.GetBatchesBySupplierIdAsync(supplierId);

        CountPending      = all.Count(b => b.Status == "Pending");
        CountTransporting = all.Count(b => b.Status == "Transporting");
        CountArrived      = all.Count(b => b.Status == "ArrivedFactory");
        CountApproved     = all.Count(b => b.Status == "Approved");

        Batches = string.IsNullOrEmpty(StatusFilter)
            ? all
            : all.Where(b => b.Status == StatusFilter).ToList();

        return Page();
    }
}
