using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Imuebles
{
    public class InmuebleRepository : IInmuebleRepository, IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _dbContextFactory;
        private ApartaPlusContext? _context;

        public InmuebleRepository(IDbContextFactory<ApartaPlusContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _context = _dbContextFactory.CreateDbContext(); // Crea una instancia del DbContext
        }

        public async Task<Inmueble> GetInmuebleByIdAsync(Guid id)
        {
            EnsureContext();
            var inmueble = await _context!.Inmuebles
                .Include(i => i.Propiedad) // Relación con Propiedad
                .Include(i => i.Contrato) // Relación con Contrato
                .Include(i => i.Facturas) // Relación con Facturas
                .FirstOrDefaultAsync(i => i.Inmuebleid == id);

            if (inmueble == null)
            {
                throw new InvalidOperationException($"Inmueble con ID {id} no encontrado.");
            }

            return inmueble;
        }

        public async Task<IEnumerable<Inmueble>> GetAllInmueblesAsync()
        {
            EnsureContext();
            return await _context!.Inmuebles
                .Include(i => i.Propiedad) // Relación con Propiedad
                .Include(i => i.Contrato) // Relación con Contrato
                .ToListAsync();
        }

        public async Task<IEnumerable<Inmueble>> GetInmueblesByEstadoAsync(bool ocupacion)
        {
            EnsureContext();
            return await _context!.Inmuebles
                .Include(i => i.Propiedad)
                .Where(i => i.Ocupacion == ocupacion)
                .ToListAsync();
        }

        public async Task AddInmuebleAsync(Inmueble inmueble)
        {
            EnsureContext();
            await _context!.Inmuebles.AddAsync(inmueble);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInmuebleAsync(Inmueble inmueble)
        {
            EnsureContext();
            var existingInmueble = await _context!.Inmuebles.FindAsync(inmueble.Inmuebleid);
            if (existingInmueble != null)
            {
                _context.Entry(existingInmueble).CurrentValues.SetValues(inmueble);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInmuebleAsync(Guid id)
        {
            EnsureContext();
            var inmueble = await _context!.Inmuebles.FindAsync(id);
            if (inmueble != null)
            {
                _context.Inmuebles.Remove(inmueble);
                await _context.SaveChangesAsync();
            }
        }

        // Asegura que el contexto está inicializado
        private void EnsureContext()
        {
            if (_context == null)
            {
                _context = _dbContextFactory.CreateDbContext();
            }
        }

        // Implementación de IAsyncDisposable
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
