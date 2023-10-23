using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class Gasto
{
    public int IdGasto { get; set; }

    public int IdUsuario { get; set; }

    public int IdCategoria { get; set; }

    public int Monto { get; set; }

    public virtual CategoriaGasto? IdCategoriaNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
