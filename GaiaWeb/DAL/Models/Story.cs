using System;
using System.Collections.Generic;

namespace GaiaWeb.DAL.Models;

public partial class Story
{
    public int StoryId { get; set; }

    public int ProductId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
