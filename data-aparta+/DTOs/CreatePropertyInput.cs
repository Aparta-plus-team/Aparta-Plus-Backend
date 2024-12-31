using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class CreatePropertyInput
    {
        public Guid? Usuarioid { get; set; }

        public string? Ubicacion { get; set; }

        public string? Nombre { get; set; }
    }
}
