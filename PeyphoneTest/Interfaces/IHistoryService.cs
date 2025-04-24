using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Interfaces
{
    public interface IHistoryService
    {
        Task<List<HistoryMovementDto>> GetHistoryByIdAsync(int walletId);
    }
}
