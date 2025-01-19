using data_aparta_.Context;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class InquilinoRepository : IInquilinoRepository, IAsyncDisposable
    {
        private readonly IDbContextFactory<ApartaPlusContext> _dbContextFactory;
        private ApartaPlusContext? _context;

        public InquilinoRepository(IDbContextFactory<ApartaPlusContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _context = _dbContextFactory.CreateDbContext(); // Crear instancia del contexto
        }

        public async Task<Inquilino?> GetInquilinoByIdAsync(Guid id)
        {
            EnsureContext();
            return await _context!.Inquilinos
                .Include(i => i.Contratos) // Incluye contratos relacionados
                .FirstOrDefaultAsync(i => i.Inquilinoid == id);
        }

        public async Task<IEnumerable<Inquilino>> GetAllInquilinosAsync()
        {
            EnsureContext();
            return await _context!.Inquilinos
                .Include(i => i.Contratos) // Incluye contratos relacionados
                .ToListAsync();
        }

        public async Task AddInquilinoAsync(Inquilino inquilino)
        {
            EnsureContext();
            await _context!.Inquilinos.AddAsync(inquilino);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInquilinoAsync(Inquilino inquilino)
        {
            EnsureContext();
            var existingInquilino = await _context!.Inquilinos.FindAsync(inquilino.Inquilinoid);
            if (existingInquilino != null)
            {
                _context.Entry(existingInquilino).CurrentValues.SetValues(inquilino);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInquilinoAsync(Guid id)
        {
            EnsureContext();
            var inquilino = await _context!.Inquilinos.FindAsync(id);
            if (inquilino != null)
            {
                _context.Inquilinos.Remove(inquilino);
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
