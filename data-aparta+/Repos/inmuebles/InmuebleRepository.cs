using data_aparta_.Context;
using data_aparta_.DTOs;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using data_aparta_.Repos.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_aparta_.Repos.Imuebles
{
      public class InmuebleRepository : IInmuebleRepository
    {
        private readonly ApartaPlusContext _context;

        public InmuebleRepository(ApartaPlusContext context)
        {
            _context = context;
        }

        public async Task<Inmueble> GetInmuebleByIdAsync(Guid id)
        {
            return await _context.Inmuebles
                .Include(i => i.Propiedad) // Relación con Propiedad
                .Include(i => i.Contrato) // Relación con Contrato
                .Include(i => i.Facturas) // Relación con Facturas
                .FirstOrDefaultAsync(i => i.Inmuebleid == id);
        }

        public async Task<IEnumerable<Inmueble>> GetAllInmueblesAsync()
        {
            return await _context.Inmuebles
                .Include(i => i.Propiedad) // Relación con Propiedad
                .Include(i => i.Contrato) // Relación con Contrato
                .ToListAsync();
        }

        public async Task<IEnumerable<Inmueble>> GetInmueblesByEstadoAsync(bool ocupacion)
        {
            return await _context.Inmuebles
                .Include(i => i.Propiedad)
                .Where(i => i.Ocupacion == ocupacion)
                .ToListAsync();
        }

        public async Task AddInmuebleAsync(Inmueble inmueble)
        {
            await _context.Inmuebles.AddAsync(inmueble);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInmuebleAsync(Inmueble inmueble)
        {
            var existingInmueble = await _context.Inmuebles.FindAsync(inmueble.Inmuebleid);
            if (existingInmueble != null)
            {
                _context.Entry(existingInmueble).CurrentValues.SetValues(inmueble);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInmuebleAsync(Guid id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble != null)
            {
                _context.Inmuebles.Remove(inmueble);
                await _context.SaveChangesAsync();
            }
        }
    }
}