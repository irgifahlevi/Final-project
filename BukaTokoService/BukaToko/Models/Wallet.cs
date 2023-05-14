using System;
using System.Collections.Generic;

namespace BukaToko.Models;

public partial class Wallet
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public int Cash { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
