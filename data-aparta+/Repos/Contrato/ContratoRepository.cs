using data_aparta_.Context;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos.Propiedades
{
    public class ContratoRepository : IContratoRepository, IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _dbContextFactory;
        private ApartaPlusContext? _context;

        public ContratoRepository(IDbContextFactory<ApartaPlusContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _context = _dbContextFactory.CreateDbContext(); // Crea una instancia de DbContext al inicializar
        }

        public async Task<Contrato?> GetContratoByIdAsync(Guid id)
        {
            EnsureContext();
            return await _context!.Contratos
                .Include(c => c.Inquilino) // Incluye la relación con Inquilino
                .Include(c => c.Inmuebles) // Incluye los inmuebles relacionados
                .FirstOrDefaultAsync(c => c.Contratoid == id);
        }

        public async Task<IEnumerable<Contrato>> GetAllContratosAsync()
        {
            EnsureContext();
            return await _context!.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmuebles)
                .ToListAsync();
        }

        public async Task AddContratoAsync(Contrato contrato)
        {
            EnsureContext();
            await _context!.Contratos.AddAsync(contrato);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContratoAsync(Contrato contrato)
        {
            EnsureContext();
            var existingContrato = await _context!.Contratos.FindAsync(contrato.Contratoid);
            if (existingContrato != null)
            {
                _context.Entry(existingContrato).CurrentValues.SetValues(contrato);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteContratoAsync(Guid id)
        {
            EnsureContext();
            var contrato = await _context!.Contratos.FindAsync(id);
            if (contrato != null)
            {
                _context.Contratos.Remove(contrato);
                await _context.SaveChangesAsync();
            }
        }

        // Método para liberar la instancia de DbContext de forma asíncrona
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
                _context = null;
            }
        }

        // Método para garantizar que el contexto exista antes de cada operación
        private void EnsureContext()
        {
            if (_context == null)
            {
                _context = _dbContextFactory.CreateDbContext();
            }
        }
    }
}
