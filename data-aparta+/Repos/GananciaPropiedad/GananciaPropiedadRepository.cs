using data_aparta_.Context;
using data_aparta_.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_aparta_.Repos
{
    public class GananciaPropiedadRepository : IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;
        private ApartaPlusContext? _context;

        public GananciaPropiedadRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
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

        public async Task<List<GananciaPropiedadDto>> GetGananciaPorPropiedad(Guid userId)
        {
            using var context = _contextFactory.CreateDbContext();

            var resultado = await context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Where(f => f.Inmueble.Propiedad.Usuarioid == userId && // Filtra por UsuarioId
                            (f.Estado == "Pagado" || f.Estado == "Por Adelantado")) // Filtra por estados válidos
                .GroupBy(f => new { f.Inmueble.Propiedad.Propiedadid, f.Inmueble.Propiedad.Nombre })
                .Select(g => new GananciaPropiedadDto
                {
                    PropiedadId = g.Key.Propiedadid,
                    NombrePropiedad = g.Key.Nombre,
                    Ganancia = g.Sum(f => f.Monto ?? 0) // Calcula la suma solo de facturas válidas
                })
                .OrderByDescending(r => r.Ganancia)
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
