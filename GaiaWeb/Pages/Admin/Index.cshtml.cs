using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaiaWeb.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ISupplierService _supplierService;
    private readonly IMaterialBatchService _batchService;
    private readonly IProductService _productService;

    public IndexModel(ISupplierService supplierService, IMaterialBatchService batchService, IProductService productService)
    {
        _supplierService = supplierService;
        _batchService = batchService;
        _productService = productService;
    }

    public string UserName { get; set; } = string.Empty;
    public int TotalSuppliers { get; set; }
    public int PendingBatches { get; set; }
    public int TotalProducts { get; set; }

    public decimal TotalWeightSaved { get; set; }
    public decimal Co2Reduction { get; set; }
    public int EquivalentTrees { get; set; }

    public Dictionary<string, decimal> MonthlyWeightCollection { get; set; } = new();
    public Dictionary<string, decimal> TopSuppliersWeight { get; set; } = new();
    public Dictionary<string, int> ProductStatusDistribution { get; set; } = new();
    public List<GaiaWeb.DAL.Models.Supplier> TopSuppliersInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Admin")
            return RedirectToPage("/Login");

        UserName = HttpContext.Session.GetString("UserName") ?? "Admin";

        var suppliers = await _supplierService.GetAllSuppliersAsync();
        TotalSuppliers = suppliers.Count;

        var batches = await _batchService.GetAllBatchesAsync();
        PendingBatches = batches.Count(b => b.Status == "Pending");

        var products = await _productService.GetAllProductsAsync();
        TotalProducts = products.Count;

        // Fetch statistics for UC22 and UC23
        TotalWeightSaved = await _batchService.GetTotalWeightSavedAsync();
        Co2Reduction = Math.Round(TotalWeightSaved * 3.15m / 1000m, 1);
        EquivalentTrees = (int)Math.Round(Co2Reduction * 15m);

        MonthlyWeightCollection = await _batchService.GetMonthlyWeightCollectionAsync();
        TopSuppliersWeight = await _batchService.GetTopSuppliersWeightAsync(5);
        ProductStatusDistribution = await _productService.GetProductStatusDistributionAsync();

        // Load active suppliers to show on list
        var activeSupplierNames = TopSuppliersWeight.Keys.Take(3).ToList();
        TopSuppliersInfo = suppliers
            .Where(s => activeSupplierNames.Contains(s.WarehouseName))
            .ToList();

        if (TopSuppliersInfo.Count < 3)
        {
            var remaining = suppliers
                .Where(s => !TopSuppliersInfo.Any(t => t.SupplierId == s.SupplierId))
                .Take(3 - TopSuppliersInfo.Count);
            TopSuppliersInfo.AddRange(remaining);
        }

        return Page();
    }
}
