using data_aparta_.Context;
using data_aparta_.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_aparta_.Repos
{
    public class ReporteVentasRepository : IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _contextFactory;
        private ApartaPlusContext? _context;

        public ReporteVentasRepository(IDbContextFactory<ApartaPlusContext> contextFactory)
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

        public async Task<List<ReporteVentasDto>> GetReporteVentasAnual(Guid userId, int year)
        {
            using var context = _contextFactory.CreateDbContext();

            var resultado = await context.Facturas
                .Include(f => f.Inmueble)
                .ThenInclude(i => i.Propiedad)
                .Where(f => f.Fechapago.HasValue &&
                            f.Fechapago.Value.Year == year && // Filtra por el año de FechaPago
                            f.Inmueble.Propiedad.Usuarioid == userId) // Filtra por UsuarioId
                .GroupBy(f => f.Fechapago.Value.Month) // Agrupa por mes
                .Select(g => new ReporteVentasDto
                {
                    Mes = g.Key, // El mes del grupo
                    Ganancia = g.Sum(f => f.Monto ?? 0) // Suma los montos
                })
                .OrderBy(r => r.Mes) // Ordena por mes
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
