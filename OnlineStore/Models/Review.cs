using System;
using System.Collections.Generic;

namespace OnlineStore.Models;

public partial class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
