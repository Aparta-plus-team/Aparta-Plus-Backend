using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Contrato
{
    public Guid Contratoid { get; set; }

    public Guid? Inquilinoid { get; set; }

    public string? Contratourl { get; set; }

    public int? Diapago { get; set; }

    public int? Mora { get; set; }

    public int? Precioalquiler { get; set; }

    public DateOnly? Fechafirma { get; set; }

    public DateOnly? Fechaterminacion { get; set; }

    public string? Fiadornombre { get; set; }

    public string? Fiadortelefono { get; set; }

    public string? Fiadorcorreo { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();

    public virtual Inquilino? Inquilino { get; set; }
}
