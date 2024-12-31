using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class RegisterIntput
    {
    public Guid Inmuebleid { get; set; }

    public Guid? Propiedadid { get; set; }

    public Guid? Contratoid { get; set; }

    public DateOnly? Fechacreacion { get; set; }

    public bool? Ocupacion { get; set; }

    public bool? Tieneparqueo { get; set; }

    public int? Numbanos { get; set; }

    public int? Numhabitaciones { get; set; }

        
    }
}