using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.DTOs
{
    public class ReporteMorosidadDto
{
    public string CodigoInmueble { get; set; }
    public int Mes { get; set; }
    public decimal? Monto { get; set; }
    public string Estado { get; set; }
    public string InquilinoNombre { get; set; }
    public string InquilinoTelefono { get; set; }
    public string InquilinoCorreo { get; set; }
}
}