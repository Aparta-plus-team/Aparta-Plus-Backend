using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Factura
{
    public Guid Facturaid { get; set; }

    public Guid? Inmuebleid { get; set; }

    public decimal? Monto { get; set; }

    public DateOnly? Fechapago { get; set; }

    public string? Estado { get; set; }

    public virtual Inmueble? Inmueble { get; set; }
}
