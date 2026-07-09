using GaiaWeb.BLL.Services.Interfaces;
using GaiaWeb.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace GaiaWeb.Pages.Supplier;

public class CollectionModel : PageModel
{
    private readonly IMaterialBatchService _batchService;
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _environment;

    public CollectionModel(
        IMaterialBatchService batchService,
        IUserRepository userRepository,
        IWebHostEnvironment environment)
    {
        _batchService = batchService;
        _userRepository = userRepository;
        _environment = environment;
    }

    [BindProperty]
    public CollectionInput Input { get; set; } = new();

    // Supplier display data (pre-populated, read-only)
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierPhone { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string DefaultAddress { get; set; } = string.Empty;
    public decimal DefaultLatitude { get; set; }
    public decimal DefaultLongitude { get; set; }
    public string NextBatchCode { get; set; } = string.Empty;
    public DateTime Now { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public class CollectionInput
    {
        [Required(ErrorMessage = "Địa chỉ thu gom không được để trống")]
        [MaxLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string CollectionAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trọng lượng không được để trống")]
        [Range(0.01, 999999.99, ErrorMessage = "Trọng lượng phải lớn hơn 0 và nhỏ hơn 1.000.000 kg")]
        public decimal WeightKg { get; set; }

        [Required(ErrorMessage = "Thời gian thu gom không được để trống")]
        public DateTime CollectionTime { get; set; } = DateTime.Now;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public IList<IFormFile>? Images { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        // Session guard: only Supplier role can access
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
        {
            return RedirectToPage("/Login");
        }

        await LoadSupplierDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Session guard
        var role = HttpContext.Session.GetString("UserRole");
        if (string.IsNullOrEmpty(role) || role != "Supplier")
        {
            return RedirectToPage("/Login");
        }

        // Reload supplier display data for re-render
        await LoadSupplierDataAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var supplierId = HttpContext.Session.GetInt32("SupplierId") ?? 0;
        if (supplierId == 0)
        {
            ErrorMessage = "Không tìm thấy thông tin nhà cung cấp trong phiên làm việc. Vui lòng đăng nhập lại.";
            return Page();
        }

        var (success, message, batchCode) = await _batchService.CreateBatchAsync(
            supplierId: supplierId,
            weightKg: Input.WeightKg,
            collectionAddress: Input.CollectionAddress,
            latitude: Input.Latitude != 0 ? Input.Latitude : DefaultLatitude,
            longitude: Input.Longitude != 0 ? Input.Longitude : DefaultLongitude,
            collectionTime: Input.CollectionTime,
            images: Input.Images ?? new List<IFormFile>(),
            webRootPath: _environment.WebRootPath
        );

        if (success)
        {
            SuccessMessage = $"{message} Mã lô: {batchCode}";
            // Reset form
            Input = new CollectionInput();
            // Re-generate next batch code
            NextBatchCode = "BAT???";
        }
        else
        {
            ErrorMessage = message;
        }

        return Page();
    }

    private async Task LoadSupplierDataAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId") ?? 0;
        var user = await _userRepository.GetByIdAsync(userId);

        if (user != null)
        {
            SupplierName = user.FullName;
            SupplierPhone = user.Phone;
            if (user.Supplier != null)
            {
                WarehouseName = user.Supplier.WarehouseName;
                DefaultAddress = user.Supplier.Address;
                DefaultLatitude = user.Supplier.Latitude;
                DefaultLongitude = user.Supplier.Longitude;
            }
        }

        Now = DateTime.Now;
        // Pre-populate input defaults
        if (Input.CollectionTime == default)
        {
            Input.CollectionTime = Now;
        }
        if (string.IsNullOrEmpty(Input.CollectionAddress))
        {
            Input.CollectionAddress = DefaultAddress;
        }
        Input.Latitude = DefaultLatitude;
        Input.Longitude = DefaultLongitude;
    }
}
