using PeyphoneTest.Models.Dtos;
using PeyphoneTest.Models;

namespace PeyphoneTest.Interfaces
{
    public interface IWalletService
    {

        Task<List<WalletDto>> GetAllWalletsAsync();
        Task<WalletDto?> GetWalletByIdAsync(int id);
        Task<int> CreateWalletAsync(WalletDto dto);
        Task<bool> UpdateWalletAsync(int id, WalletDto dto);
        Task<bool> DeleteWalletAsync(int id);
        Task<(bool Success, string Message)> TransferAsync(TransferDto dto);

    }
}
