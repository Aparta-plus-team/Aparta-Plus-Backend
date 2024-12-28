using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Propiedad
{
    public Guid Propiedadid { get; set; }

    public Guid? Usuarioid { get; set; }

    public string? Ubicacion { get; set; }

    public string? Nombre { get; set; }

    public string? Portadaurl { get; set; }

    public DateOnly? Fechacreacion { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Imagenespropiedade> Imagenespropiedades { get; set; } = new List<Imagenespropiedade>();

    public virtual ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();

    public virtual Usuario? Usuario { get; set; }
}
