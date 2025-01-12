using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;

namespace aparta_.Services
{
    public class DashboardStatisticsService
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IContratoRepository _contratoRepository;
        private readonly IInquilinoRepository _inquilinoRepository;

        public DashboardStatisticsService(
            IInmuebleRepository inmuebleRepository,
            IContratoRepository contratoRepository,
            IInquilinoRepository inquilinoRepository)
        {
            _inmuebleRepository = inmuebleRepository;
            _contratoRepository = contratoRepository;
            _inquilinoRepository = inquilinoRepository;
        }

public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(Guid propiedadId)
{
    try
    {
        // Obtener todos los inmuebles
        var inmuebles = await _inmuebleRepository.GetAllInmueblesAsync();
        var inmueblesPropiedad = inmuebles.Where(i => i.Propiedadid == propiedadId).ToList();

        int totalInmuebles = inmueblesPropiedad.Count;
        int totalInmueblesDesocupados = inmueblesPropiedad.Count(i => i.Ocupacion.HasValue && !i.Ocupacion.Value);

        // Obtener todos los contratos
        var contratos = await _contratoRepository.GetAllContratosAsync();
        var contratosPropiedad = contratos
            .Where(c => inmueblesPropiedad.Any(i => i.Contratoid == c.Contratoid))
            .ToList();

        double duracionPromedioContratos = contratosPropiedad.Any()
            ? contratosPropiedad.Average(c =>
                (c.Fechaterminacion.HasValue && c.Fechafirma.HasValue)
                    ? (c.Fechaterminacion.Value.ToDateTime(TimeOnly.MinValue) - c.Fechafirma.Value.ToDateTime(TimeOnly.MinValue)).TotalDays / 30
                    : 0)
            : 0;

        decimal ingresoMensual = contratosPropiedad.Sum(c => c.Precioalquiler ?? 0);

        // Obtener inquilinos
        var inquilinos = await _inquilinoRepository.GetAllInquilinosAsync();
        int totalHombres = inquilinos.Count(i => i.Inquilinogenero.HasValue && !i.Inquilinogenero.Value); // 0 = Hombre
        int totalMujeres = inquilinos.Count(i => i.Inquilinogenero.HasValue && i.Inquilinogenero.Value); // 1 = Mujer

        // Otros cálculos
        double porcentajeOcupacion = totalInmuebles > 0
            ? ((totalInmuebles - totalInmueblesDesocupados) / (double)totalInmuebles) * 100
            : 0;

        return new DashboardStatisticsDTO
        {
            TotalInmuebles = totalInmuebles,
            TotalInmueblesDesocupados = totalInmueblesDesocupados,
            DuracionPromedioContratos = duracionPromedioContratos,
            IngresoMensualPropiedades = ingresoMensual,
            TotalHombres = totalHombres,
            TotalMujeres = totalMujeres,
            PorcentajeOcupacion = porcentajeOcupacion
        };
    }
    catch (Exception ex)
    {
        // Captura la excepción y lanza un error más claro
        throw new Exception($"Error al obtener estadísticas: {ex.Message}");
    }
}

    }
}
