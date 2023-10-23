using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class Ahorro
{
    public int IdAhorro { get; set; }

    public int IdUsuario { get; set; }

    public int IdMeta { get; set; }

    public int Monto { get; set; }

    public virtual MetaAhorro? IdMetaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
