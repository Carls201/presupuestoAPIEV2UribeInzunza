using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string Rol1 { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
