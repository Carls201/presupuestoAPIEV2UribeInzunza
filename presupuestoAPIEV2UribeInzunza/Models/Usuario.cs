using System;
using System.Collections.Generic;

namespace presupuestoAPIEV2UribeInzunza.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public int Edad { get; set; }

    public string Direccion { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Pass { get; set; } = null!;

    public virtual ICollection<Ahorro> Ahorros { get; set; } = new List<Ahorro>();

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual Rol? IdRolNavigation { get; set; }

    public virtual ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
}
