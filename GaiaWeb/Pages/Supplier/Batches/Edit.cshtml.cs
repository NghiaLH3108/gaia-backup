using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GaiaWeb.Pages.Supplier.Batches;

public class EditModel : PageModel
{
    private readonly IMaterialBatchService _batchService;

    public EditModel(IMaterialBatchService batchService)
    {
        _batchService = batchService;
    }

    [BindProperty(SupportsGet = true)]
    public int BatchId { get; set; }

    [BindProperty]
    public EditInput Input { get; set; } = new();

    public MaterialBatch? Batch { get; set; }

    public string SupplierName  { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage   { get; set; }

    public class EditInput
    {
        [Required(ErrorMessage = "Địa chỉ thu gom không được để trống")]
        [MaxLength(255)]
        public string CollectionAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trọng lượng không được để trống")]
        [Range(0.01, 999999.99, ErrorMessage = "Trọng lượng phải lớn hơn 0")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Thời gian thu gom không được để trống")]
        public DateTime CollectionTime { get; set; } = DateTime.Now;
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
        var (success, message) = await _batchService.UpdateBatchAsync(
            BatchId,
            supplierId,
            Input.CollectionAddress,
            Input.WeightKg,
            Input.CollectionTime);

        if (success)
        {
            TempData["SuccessMessage"] = $"Cập nhật lô hàng #{BatchId} thành công.";
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
        {
            TempData["SuccessMessage"] = null;
            return RedirectToPage("/Supplier/Batches/Index");
        }

        // Pre-fill form on GET
        if (HttpContext.Request.Method == "GET")
        {
            Input.CollectionAddress = Batch.CollectionAddress;
            Input.WeightKg          = Batch.WeightKg;
            Input.CollectionTime    = Batch.CollectionTime;
        }

        return Page();
    }
}
