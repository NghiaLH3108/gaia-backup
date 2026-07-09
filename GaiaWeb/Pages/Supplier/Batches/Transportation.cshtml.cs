using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GaiaWeb.Pages.Supplier.Batches;

public class TransportationModel : PageModel
{
    private readonly IMaterialBatchService _batchService;

    public TransportationModel(IMaterialBatchService batchService)
    {
        _batchService = batchService;
    }

    [BindProperty(SupportsGet = true)]
    public int BatchId { get; set; }

    [BindProperty]
    public TransportInput Input { get; set; } = new();

    public MaterialBatch? Batch { get; set; }
    public string SupplierName  { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    public class TransportInput
    {
        [Required(ErrorMessage = "Vui lòng chọn trạng thái vận chuyển")]
        public string Status { get; set; } = "Transporting";

        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        [MaxLength(500, ErrorMessage = "Mô tả không vượt quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
            return RedirectToPage("/Login");

        LoadSessionInfo();
        return await LoadBatchAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
            return RedirectToPage("/Login");

        LoadSessionInfo();

        if (!ModelState.IsValid)
        {
            await LoadBatchAsync();
            return Page();
        }

        var supplierId = HttpContext.Session.GetInt32("SupplierId") ?? 0;
        var (success, message) = await _batchService.AddTransportationStatusAsync(
            BatchId, supplierId, Input.Status, Input.Description);

        if (success)
        {
            TempData["SuccessMessage"] = $"Cập nhật trạng thái vận chuyển lô @{BatchId} thành công.";
            return RedirectToPage("/Supplier/Batches/Index");
        }

        ErrorMessage = message;
        await LoadBatchAsync();
        return Page();
    }

    private void LoadSessionInfo()
    {
        SupplierName  = HttpContext.Session.GetString("UserName") ?? "";
        WarehouseName = HttpContext.Session.GetString("WarehouseName") ?? "";
    }

    private async Task<IActionResult> LoadBatchAsync()
    {
        var supplierId = HttpContext.Session.GetInt32("SupplierId") ?? 0;
        Batch = await _batchService.GetBatchDetailAsync(BatchId, supplierId);
        if (Batch == null)
            return RedirectToPage("/Supplier/Batches/Index");
        return Page();
    }
}
