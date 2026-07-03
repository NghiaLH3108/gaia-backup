using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class MaterialBatch
{
    public int BatchId { get; set; }

    public int SupplierId { get; set; }

    public string BatchCode { get; set; } = null!;

    public decimal WeightKg { get; set; }

    public string CollectionAddress { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime CollectionTime { get; set; }

    public DateTime? ApprovedTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<MaterialImage> MaterialImages { get; set; } = new List<MaterialImage>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual ICollection<TransportationHistory> TransportationHistories { get; set; } = new List<TransportationHistory>();
}
