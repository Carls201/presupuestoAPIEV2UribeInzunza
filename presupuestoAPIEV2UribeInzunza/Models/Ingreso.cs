using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class Ingreso
{
    public int IdIngreso { get; set; }

    public int IdUsuario { get; set; }

    public int IdFuente { get; set; }

    public int Monto { get; set; }

    public virtual FuenteIngreso? IdFuenteNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
