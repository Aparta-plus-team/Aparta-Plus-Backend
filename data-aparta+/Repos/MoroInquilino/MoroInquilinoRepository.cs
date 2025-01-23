using data_aparta_.Context;
using data_aparta_.DTOs;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class MorosidadPorPropiedadRepository : IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;
        private ApartaPlusContext? _context;

        public MorosidadPorPropiedadRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private ApartaPlusContext Context
        {
            get
            {
                if (_context == null)
                {
                    _context = _contextFactory.CreateDbContext();
                }
                return _context;
            }
        }

        public async Task<List<MorosidadPorPropiedadDto>> GetMorosidadPorUsuarioAsync(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            var resultado = await context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Include(f => f.Inmueble.Contrato)
                .ThenInclude(c => c.Inquilino)
                .Where(f => f.Inmueble.Propiedad.Usuarioid == userId && // Filtra por UsuarioId
                            f.Estado == "Atrasado") // Filtra por el estado "Atrasado"
                .GroupBy(f => new
                {
                    PropiedadNombre = f.Inmueble.Propiedad.Nombre,
                    InquilinoNombre = f.Inmueble.Contrato.Inquilino.Inquilinonombre
                })
                .Select(g => new MorosidadPorPropiedadDto
                {
                    PropiedadNombre = g.Key.PropiedadNombre,
                    InquilinoNombre = g.Key.InquilinoNombre ?? "Sin inquilino"
                })
                .OrderBy(r => r.PropiedadNombre)
                .ThenBy(r => r.InquilinoNombre)
                .ToListAsync();

            return resultado;
        }

        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
                _context = null;
            }
        }
    }
}
