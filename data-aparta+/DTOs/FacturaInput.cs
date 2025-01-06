using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class FacturaDTO
    {
        public Guid Facturaid { get; set; }
        public Guid? Inmuebleid { get; set; }
        public decimal? Monto { get; set; }
        public DateOnly? Fechapago { get; set; }
        public string? Estado { get; set; }
    }
}
