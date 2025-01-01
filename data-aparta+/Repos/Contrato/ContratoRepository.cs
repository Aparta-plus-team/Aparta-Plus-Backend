using data_aparta_.Context;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos.Propiedades
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly ApartaPlusContext _context;

        public ContratoRepository(ApartaPlusContext context)
        {
            _context = context;
        }

        public async Task<Contrato?> GetContratoByIdAsync(Guid id)
        {
            return await _context.Contratos
                .Include(c => c.Inquilino) // Incluye la relación con Inquilino
                .Include(c => c.Inmuebles) // Incluye los inmuebles relacionados
                .FirstOrDefaultAsync(c => c.Contratoid == id);
        }

        public async Task<IEnumerable<Contrato>> GetAllContratosAsync()
        {
            return await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmuebles)
                .ToListAsync();
        }

        public async Task AddContratoAsync(Contrato contrato)
        {
            await _context.Contratos.AddAsync(contrato);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContratoAsync(Contrato contrato)
        {
            var existingContrato = await _context.Contratos.FindAsync(contrato.Contratoid);
            if (existingContrato != null)
            {
                _context.Entry(existingContrato).CurrentValues.SetValues(contrato);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteContratoAsync(Guid id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato != null)
            {
                _context.Contratos.Remove(contrato);
                await _context.SaveChangesAsync();
            }
        }
    }
}
