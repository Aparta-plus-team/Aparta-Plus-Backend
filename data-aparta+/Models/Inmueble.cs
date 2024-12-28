using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Inmueble
{
    public Guid Inmuebleid { get; set; }

    public Guid? Propiedadid { get; set; }

    public Guid? Contratoid { get; set; }

    public string? Codigo { get; set; }

    public DateOnly? Fechacreacion { get; set; }

    public bool? Ocupacion { get; set; }

    public bool? Tieneparqueo { get; set; }

    public int? Numbanos { get; set; }

    public int? Numhabitaciones { get; set; }

    public virtual Contrato? Contrato { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();

    public virtual Propiedad? Propiedad { get; set; }
}
