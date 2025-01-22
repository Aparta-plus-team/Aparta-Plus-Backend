using data_aparta_.Context;
using data_aparta_.DTOs;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class EstadisticaFinancieraRepository
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;

        public EstadisticaFinancieraRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<EstadisticaFinancieraDto>> GetEstadisticaFinancieraAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            var resultado = await context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Where(f => f.Inmueble.Propiedad.Usuarioid == userId)
                .GroupBy(f => new
                {
                    Mes = f.Fechapago.HasValue ? f.Fechapago.Value.Month : 0,
                    Anio  = f.Fechapago.HasValue ? f.Fechapago.Value.Year : 0
                })
                .Select(g => new EstadisticaFinancieraDto
                {
                    Mes = g.Key.Mes,
                    Anio  = g.Key.Anio ,
                    TotalGanancias = g.Where(f => f.Estado == "Pagado").Sum(f => f.Monto ?? 0),
                    TotalDeudas = g.Where(f => f.Estado == "No Pagado" || f.Estado == "Atrasada").Sum(f => f.Monto ?? 0)
                })
                .OrderBy(r => r.Anio )
                .ThenBy(r => r.Mes)
                .ToListAsync();

            return resultado;
        }
    }
}
