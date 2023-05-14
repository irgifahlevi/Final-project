using System;
using System.Collections.Generic;

namespace AdaDanaService.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Saldo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
