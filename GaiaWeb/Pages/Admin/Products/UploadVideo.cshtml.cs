using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GaiaWeb.Pages.Admin.Products;

public class UploadVideoModel : PageModel
{
    private readonly IProductService _productService;
    private readonly IWebHostEnvironment _env;

    public UploadVideoModel(IProductService productService, IWebHostEnvironment env)
    {
        _productService = productService;
        _env = env;
    }

    [BindProperty(SupportsGet = true)]
    public int? ProductId { get; set; }

    [BindProperty]
    public int SelectedTimelineId { get; set; }

    [BindProperty]
    public string VideoTitle { get; set; } = string.Empty;

    [BindProperty]
    public string ArtisanName { get; set; } = string.Empty;

    [BindProperty]
    public string ProcessDescription { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? VideoFile { get; set; }

    public List<SelectListItem> ProductOptions { get; set; } = new();
    public List<ProductTimeline> Timelines { get; set; } = new();
    public string? SelectedProductName { get; set; }
    public string? UserName { get; set; }
    
    public string? Message { get; set; }
    public bool IsSuccess { get; set; } = true;

    public async Task<IActionResult> OnGetAsync(string? msg, bool? ok)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        UserName = HttpContext.Session.GetString("UserName") ?? "Admin";
        
        if (msg != null)
        {
            Message = msg;
            IsSuccess = ok ?? true;
        }

        await LoadProducts();

        if (ProductId.HasValue)
        {
            var product = await _productService.GetProductByIdAsync(ProductId.Value);
            if (product != null)
            {
                SelectedProductName = product.ProductName;
                Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
            }
        }
        else if (ProductOptions.Any())
        {
            // Default to first product if none specified
            ProductId = int.Parse(ProductOptions.First().Value);
            var product = await _productService.GetProductByIdAsync(ProductId.Value);
            if (product != null)
            {
                SelectedProductName = product.ProductName;
                Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin")
            return RedirectToPage("/Login");

        UserName = HttpContext.Session.GetString("UserName") ?? "Admin";

        if (SelectedTimelineId <= 0)
        {
            Message = "Vui lòng chọn công đoạn của sản phẩm.";
            IsSuccess = false;
            await LoadProducts();
            if (ProductId.HasValue)
            {
                Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
            }
            return Page();
        }

        var timeline = await _productService.GetTimelineByIdAsync(SelectedTimelineId);
        if (timeline == null)
        {
            Message = "Không tìm thấy công đoạn này.";
            IsSuccess = false;
            await LoadProducts();
            return Page();
        }

        string videoUrl = timeline.VideoUrl;

        if (VideoFile != null && VideoFile.Length > 0)
        {
            var ext = Path.GetExtension(VideoFile.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm" };
            if (!allowedExtensions.Contains(ext))
            {
                Message = "Định dạng video không được hỗ trợ. Chỉ hỗ trợ MP4, MOV, AVI, MKV, WEBM.";
                IsSuccess = false;
                await LoadProducts();
                if (ProductId.HasValue)
                {
                    Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
                }
                return Page();
            }

            if (VideoFile.Length > 500 * 1024 * 1024) // 500MB limit
            {
                Message = "Dung lượng video vượt quá giới hạn 500MB.";
                IsSuccess = false;
                await LoadProducts();
                if (ProductId.HasValue)
                {
                    Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
                }
                return Page();
            }

            try
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "videos", "uploads");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var uniqueName = $"{timeline.ProductId}_{timeline.StepOrder}_{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadDir, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await VideoFile.CopyToAsync(stream);
                }

                videoUrl = $"/videos/uploads/{uniqueName}";
            }
            catch (Exception ex)
            {
                Message = $"Lỗi khi lưu trữ video: {ex.Message}";
                IsSuccess = false;
                await LoadProducts();
                if (ProductId.HasValue)
                {
                    Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
                }
                return Page();
            }
        }

        // Format description to include Artisan Name if provided
        string finalDescription = ProcessDescription;
        if (!string.IsNullOrWhiteSpace(ArtisanName))
        {
            finalDescription = $"Nghệ nhân: {ArtisanName.Trim()}\n{ProcessDescription}";
        }

        var (success, serviceMsg) = await _productService.UpdateTimelineVideoAsync(
            SelectedTimelineId, VideoTitle, finalDescription, videoUrl);

        if (success)
        {
            return RedirectToPage(new { productId = ProductId, msg = serviceMsg, ok = true });
        }
        else
        {
            Message = serviceMsg;
            IsSuccess = false;
            await LoadProducts();
            if (ProductId.HasValue)
            {
                Timelines = await _productService.GetProductTimelinesByProductIdAsync(ProductId.Value);
            }
            return Page();
        }
    }

    private async Task LoadProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        ProductOptions = products.Select(p => new SelectListItem
        {
            Value = p.ProductId.ToString(),
            Text = p.ProductName,
            Selected = p.ProductId == ProductId
        }).ToList();
    }
}
