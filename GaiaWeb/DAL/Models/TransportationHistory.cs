using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class TransportationHistory
{
    public int HistoryId { get; set; }

    public int BatchId { get; set; }

    public string Status { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime UpdateTime { get; set; }

    public virtual MaterialBatch Batch { get; set; } = null!;
}
