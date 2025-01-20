using data_aparta_.DTOs;
using data_aparta_.Repos.Contracts;

namespace aparta_.Services
{
    public class DashboardStatisticsService
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IContratoRepository _contratoRepository;
        private readonly IInquilinoRepository _inquilinoRepository;
        private readonly IPropertyRepository _propiedadRepository;

        public DashboardStatisticsService(
            IInmuebleRepository inmuebleRepository,
            IContratoRepository contratoRepository,
            IInquilinoRepository inquilinoRepository,
            IPropertyRepository propertyRepository
        )
        {
            _inmuebleRepository = inmuebleRepository;
            _contratoRepository = contratoRepository;
            _inquilinoRepository = inquilinoRepository;
            _propiedadRepository = propertyRepository;
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(Guid userId)
        {
            try
            {
                // **RESUMEN SUPERIOR (Propiedades)**
                var propiedades = await _propiedadRepository.GetPropiedadesByUsuarioId(userId);
                int totalPropiedades = propiedades.Count;

                // Obtener inmuebles asociados a las propiedades
                var inmuebles = await _inmuebleRepository.GetAllInmueblesAsync();
                var inmueblesUsuario = inmuebles.Where(i => propiedades.Any(p => p.Propiedadid == i.Propiedadid)).ToList();

                // Calcular propiedades alquiladas (inmuebles con Contratoid asociado)
                int propiedadesAlquiladas = inmueblesUsuario.Count(i => i.Ocupacion.HasValue);

                // Obtener contratos asociados a los inmuebles del usuario
                var contratos = await _contratoRepository.GetAllContratosAsync();
                var contratosUsuario = contratos
                    .Where(c => inmueblesUsuario.Any(i => i.Contratoid == c.Contratoid))
                    .ToList();

                decimal gananciaMensual = contratosUsuario.Sum(c => c.Precioalquiler ?? 0);

                // **ESTADÍSTICAS DE GANANCIAS MENSUALES**
                var estadisticasMensuales = contratosUsuario
                    .GroupBy(c => c.Fechafirma.HasValue ? c.Fechafirma.Value.ToDateTime(TimeOnly.MinValue).ToString("MMMM yyyy") : "Desconocido")
                    .Select(g => new GananciaMensualDto
                    {
                        Mes = g.Key,
                        Ganancia = g.Sum(c => c.Precioalquiler ?? 0)
                    })
                    .ToList();

                // **DESGLOSE POR UBICACIÓN (Inmuebles)**
                var desgloseUbicacion = inmueblesUsuario
                    .GroupBy(i => propiedades.FirstOrDefault(p => p.Propiedadid == i.Propiedadid)?.Ubicacion ?? "Desconocida")
                    .Select(g => new DesgloseUbicacionDto
                    {
                        Ubicacion = g.Key,
                        Ganancia = g.Sum(i => contratosUsuario
                            .Where(c => c.Contratoid == i.Contratoid)
                            .Sum(c => c.Precioalquiler ?? 0)),
                        Porcentaje = contratosUsuario.Sum(c => c.Precioalquiler ?? 0) > 0
                            ? (g.Sum(i => contratosUsuario
                                .Where(c => c.Contratoid == i.Contratoid)
                                .Sum(c => c.Precioalquiler ?? 0)) / contratosUsuario.Sum(c => c.Precioalquiler ?? 0)) * 100
                            : 0
                    })
                    .ToList();

                // **TRANSACCIONES RECIENTES**
                var transaccionesRecientes = contratosUsuario
                    .OrderByDescending(c => c.Fechafirma)
                    .Take(5)
                    .Select(c => new TransaccionRecienteDto
                    {
                        PropiedadNombre = propiedades.FirstOrDefault(p => inmueblesUsuario.Any(i => i.Propiedadid == p.Propiedadid && i.Contratoid == c.Contratoid))?.Nombre ?? "Desconocida",
                        Fecha = c.Fechafirma.HasValue ? c.Fechafirma.Value.ToDateTime(TimeOnly.MinValue) : DateTime.MinValue,
                        Monto = c.Precioalquiler ?? 0
                    })
                    .ToList();

                // **MOROSIDADES**
                var morosidades = inmueblesUsuario
                    .Where(i => i.Ocupacion.HasValue && !i.Ocupacion.Value)
                    .Select(i => new MorosidadDto
                    {
                        Servicio = "Propiedad Desocupada",
                        Detalle = "Oportunidad perdida por desocupación",
                        PropiedadNombre = propiedades.FirstOrDefault(p => p.Propiedadid == i.Propiedadid)?.Nombre ?? "Desconocida",
                        Inquilino = "Sin inquilino"
                    })
                    .ToList();

                // **RETORNAR DTO COMPLETO**
                return new DashboardStatisticsDTO
                {
                    TotalPropiedades = totalPropiedades,
                    PropiedadesAlquiladas = propiedadesAlquiladas,
                    GananciaMensual = gananciaMensual,
                    GananciasMensuales = estadisticasMensuales,
                    DesglosePorUbicacion = desgloseUbicacion,
                    TransaccionesRecientes = transaccionesRecientes,
                    Morosidades = morosidades
                };
            }
            catch (Exception ex)
            {
                // Captura la excepción y lanza un error más claro
                throw new Exception($"Error al obtener estadísticas del dashboard: {ex.Message}");
            }
        }
    }
}
