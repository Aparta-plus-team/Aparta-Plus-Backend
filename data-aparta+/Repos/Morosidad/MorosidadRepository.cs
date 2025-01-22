using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class ReporteMorosidadRepository
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;

        public ReporteMorosidadRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ReporteMorosidadDto>> GetReporteMorosidadPorUsuarioPropiedadYAnioAsync(
    Guid userId,
    Guid? propertyId,
    int anio)
        {
            // Crear el contexto
            using var context = _contextFactory.CreateDbContext();

            // Construir la consulta base
            var query = context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Contrato)
                .ThenInclude(c => c.Inquilino)
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Where(f => f.Fechapago.HasValue && f.Fechapago.Value.Year == anio && f.Inmueble.Propiedad.Usuarioid == userId);

            // Aplicar el filtro de propiedad solo si propertyId tiene valor
            if (propertyId.HasValue)
            {
                query = query.Where(f => f.Inmueble.Propiedad.Propiedadid == propertyId.Value);
            }

            // Ejecutar la consulta y proyectar los resultados
            return await query
                .Select(f => new ReporteMorosidadDto
                {
                    CodigoInmueble = f.Inmueble.Codigo,
                    Mes = f.Fechapago.Value.Month,
                    Monto = f.Monto,
                    Estado = f.Estado,
                    InquilinoNombre = f.Inmueble.Contrato.Inquilino.Inquilinonombre,
                    InquilinoTelefono = f.Inmueble.Contrato.Inquilino.Inquilinotelefono,
                    InquilinoCorreo = f.Inmueble.Contrato.Inquilino.Inquilinocorreo
                })
                .OrderBy(r => r.Mes)
                .ToListAsync();
        }
    }
}
