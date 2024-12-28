using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Inquilino
{
    public Guid Inquilinoid { get; set; }

    public string? Inquilinonombre { get; set; }

    public string? Inquilinotelefono { get; set; }

    public string? Inquilinocorreo { get; set; }

    public bool? Inquilinogenero { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Contrato> Contratos { get; set; } = new List<Contrato>();
}
