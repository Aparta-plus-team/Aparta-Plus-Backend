using System;
using System.Collections.Generic;

namespace data_aparta_.Models;

public partial class Usuario
{
    public Guid Usuarioid { get; set; }

    public string? Usuarionombre { get; set; }

    public string? Usuariocorreo { get; set; }

    public string? Usuariotelefono { get; set; }

    public string? Usuariohash { get; set; }

    public string? Refreshtoken { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Propiedad> Propiedads { get; set; } = new List<Propiedad>();
}
