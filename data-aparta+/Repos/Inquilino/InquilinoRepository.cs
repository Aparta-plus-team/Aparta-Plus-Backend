using data_aparta_.Context;
using data_aparta_.Models;
using data_aparta_.Repos.Contracts;
using Microsoft.EntityFrameworkCore;

namespace data_aparta_.Repos
{
    public class InquilinoRepository : IInquilinoRepository
    {
        private readonly ApartaPlusContext _context;

        public InquilinoRepository(ApartaPlusContext context)
        {
            _context = context;
        }

        public async Task<Inquilino?> GetInquilinoByIdAsync(Guid id)
        {
            return await _context.Inquilinos
                .Include(i => i.Contratos) // Incluye contratos relacionados
                .FirstOrDefaultAsync(i => i.Inquilinoid == id);
        }

        public async Task<IEnumerable<Inquilino>> GetAllInquilinosAsync()
        {
            return await _context.Inquilinos
                .Include(i => i.Contratos) // Incluye contratos relacionados
                .ToListAsync();
        }

        public async Task AddInquilinoAsync(Inquilino inquilino)
        {
            await _context.Inquilinos.AddAsync(inquilino);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInquilinoAsync(Inquilino inquilino)
        {
            var existingInquilino = await _context.Inquilinos.FindAsync(inquilino.Inquilinoid);
            if (existingInquilino != null)
            {
                _context.Entry(existingInquilino).CurrentValues.SetValues(inquilino);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteInquilinoAsync(Guid id)
        {
            var inquilino = await _context.Inquilinos.FindAsync(id);
            if (inquilino != null)
            {
                _context.Inquilinos.Remove(inquilino);
                await _context.SaveChangesAsync();
            }
        }
    }
}
