using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Imagenespropiedade
{
    public Guid Imagenid { get; set; }

    public Guid? Propiedadid { get; set; }

    public string? Imagenurl { get; set; }

    public DateOnly? Fechacreacion { get; set; }

    public bool? Estado { get; set; }

    public virtual Propiedad? Propiedad { get; set; }
}
