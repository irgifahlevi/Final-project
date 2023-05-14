using System;
using System.Collections.Generic;

namespace Goole_OpenId.Models;

public partial class Role
{
    public int Id { get; set; }

    public string NameRole { get; set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
