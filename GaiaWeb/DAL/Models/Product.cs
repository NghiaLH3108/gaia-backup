using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int BatchId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid Qrtoken { get; set; }

    public string CurrentStatus { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual MaterialBatch Batch { get; set; } = null!;

    public virtual ICollection<ProductTimeline> ProductTimelines { get; set; } = new List<ProductTimeline>();

    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();
}
