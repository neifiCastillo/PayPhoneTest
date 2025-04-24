using Microsoft.EntityFrameworkCore;
using PeyphoneTest.Interfaces;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Services
{
    public class HistoryService : IHistoryService
    {

        private readonly DbpayphoneTestContext _context;

        public HistoryService(DbpayphoneTestContext context)
        {
            _context = context;
        }

        public async Task<List<HistoryMovementDto>> GetHistoryByIdAsync(int walletId)
        {
            return await _context.HistoryMovements
                                 .Where(u => u.WalletId == walletId).Select(u => new HistoryMovementDto
                                 {
                                     Amount = u.Amount,
                                     Type = u.Type,
                                     CreatedAt = u.CreatedAt

                                 }).ToListAsync();


        }

    }
}
