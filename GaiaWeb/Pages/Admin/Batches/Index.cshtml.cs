using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GaiaWeb.Pages.Admin.Batches;

public class IndexModel : PageModel
{
    private readonly IMaterialBatchService _batchService;
    public IndexModel(IMaterialBatchService batchService) => _batchService = batchService;

    public List<MaterialBatch> Batches { get; set; } = new();
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public string Filter { get; set; } = "All";

    public async Task<IActionResult> OnGetAsync(string? filter, string? msg, bool? ok)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        Filter = filter ?? "All";
        Message = msg;
        IsSuccess = ok ?? true;

        var all = await _batchService.GetAllBatchesAsync();
        Batches = Filter == "Pending"
            ? all.Where(b => b.Status == "Pending").ToList()
            : all;

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int batchId, string? filter)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _batchService.ApproveBatchAsync(batchId);
        return RedirectToPage(new { filter, msg = message, ok = success });
    }

    public async Task<IActionResult> OnPostRejectAsync(int batchId, string? filter)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        var (success, message) = await _batchService.RejectBatchAsync(batchId);
        return RedirectToPage(new { filter, msg = message, ok = success });
    }
}
