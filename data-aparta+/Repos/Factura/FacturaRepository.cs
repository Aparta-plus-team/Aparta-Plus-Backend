using data_aparta_.Context;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly ApartaPlusContext _context;

        public FacturaRepository(ApartaPlusContext context)
        {
            _context = context;
        }

        public async Task<Factura?> GetFacturaByIdAsync(Guid id)
        {
            return await _context.Facturas
                .Include(f => f.Inmueble) // Include related Inmueble data
                .FirstOrDefaultAsync(f => f.Facturaid == id);
        }

        public async Task<IEnumerable<Factura>> GetAllFacturasAsync()
        {
            return await _context.Facturas
                .Include(f => f.Inmueble) // Include related Inmueble data
                .ToListAsync();
        }

        public async Task AddFacturaAsync(Factura factura)
        {
            await _context.Facturas.AddAsync(factura);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFacturaAsync(Factura factura)
        {
            var existingFactura = await _context.Facturas.FindAsync(factura.Facturaid);
            if (existingFactura != null)
            {
                _context.Entry(existingFactura).CurrentValues.SetValues(factura);
                await _context.SaveChangesAsync();
            }
        }
    }
}
