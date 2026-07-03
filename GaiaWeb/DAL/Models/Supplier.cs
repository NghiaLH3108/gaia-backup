using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public int UserId { get; set; }

    public string WarehouseName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<MaterialBatch> MaterialBatches { get; set; } = new List<MaterialBatch>();

    public virtual User User { get; set; } = null!;
}
