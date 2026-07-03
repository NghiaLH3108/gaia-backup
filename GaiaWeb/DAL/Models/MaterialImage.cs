using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class MaterialImage
{
    public int ImageId { get; set; }

    public int BatchId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime UploadTime { get; set; }

    public virtual MaterialBatch Batch { get; set; } = null!;
}
