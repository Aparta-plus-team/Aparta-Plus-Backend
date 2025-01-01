using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{

    public class InquilinoDto
    {
        public Guid Inquilinoid { get; set; }
        public string? Inquilinonombre { get; set; }
        public string? Inquilinotelefono { get; set; }
        public string? Inquilinocorreo { get; set; }
        public bool? Inquilinogenero { get; set; } // 0: Masculino, 1: Femenino
        public bool? Estado { get; set; }

        // Relaciones
        public ICollection<ContratoDto>? Contratos { get; set; }
    }

    public class CreateInquilinoDto
    {
        public string? Inquilinonombre { get; set; }
        public string? Inquilinotelefono { get; set; }
        public string? Inquilinocorreo { get; set; }
        public bool? Inquilinogenero { get; set; }
        public bool? Estado { get; set; }
    }

    public class UpdateInquilinoDto
    {
        public string? Inquilinonombre { get; set; }
        public string? Inquilinotelefono { get; set; }
        public string? Inquilinocorreo { get; set; }
        public bool? Inquilinogenero { get; set; }
        public bool? Estado { get; set; }
    }




}