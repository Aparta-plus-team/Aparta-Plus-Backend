using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class InquilinoDeudaRepository
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;

        public InquilinoDeudaRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<InquilinoDeudaDto>> GetInquilinosConDeudasAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            var resultado = await context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Include(f => f.Inmueble.Contrato)
                .ThenInclude(c => c.Inquilino)
                .Where(f => (f.Estado == "No Pagado" || f.Estado == "Cancelado") 
                            && f.Inmueble.Propiedad.Usuarioid == userId)
                .GroupBy(f => new 
                {
                    f.Inmueble.Contrato.Inquilino.Inquilinoid,
                    f.Inmueble.Contrato.Inquilino.Inquilinonombre,
                    f.Inmueble.Contrato.Inquilino.Inquilinocorreo,
                    f.Estado
                })
                .Select(g => new InquilinoDeudaDto
                {
                    InquilinoId = g.Key.Inquilinoid,
                    Nombre = g.Key.Inquilinonombre,
                    Correo = g.Key.Inquilinocorreo,
                    EstadoFactura = g.Key.Estado,
                    CantidadFacturas = g.Count(),
                    TotalDeuda = g.Sum(f => f.Monto ?? 0)
                })
                .OrderByDescending(r => r.TotalDeuda)
                .ToListAsync();

            return resultado;
        }
    }
}
