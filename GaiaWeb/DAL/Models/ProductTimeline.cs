using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class ProductTimeline
{
    public int TimelineId { get; set; }

    public int ProductId { get; set; }

    public int StepOrder { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Location { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string VideoUrl { get; set; } = null!;

    public DateTime TimelineTime { get; set; }

    public virtual Product Product { get; set; } = null!;
}
