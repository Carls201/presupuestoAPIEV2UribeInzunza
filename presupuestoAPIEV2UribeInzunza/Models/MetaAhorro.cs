using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class MetaAhorro
{
    public int IdMeta { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Ahorro> Ahorros { get; set; } = new List<Ahorro>();
}
