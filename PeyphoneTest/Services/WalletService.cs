using PeyphoneTest.Interfaces;
using Microsoft.EntityFrameworkCore;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Services
{
    public class WalletService : IWalletService
    {
        private readonly DbpayphoneTestContext _context;

        public WalletService(DbpayphoneTestContext context)
        {
            _context = context;
        }

        public async Task<List<Wallet>> GetAllWalletsAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<Wallet?> GetWalletByIdAsync(int id)
        {
            return await _context.Wallets.FindAsync(id);
        }

        public async Task<int> CreateWalletAsync(WalletDto dto)
        {
            var wallet = new Wallet
            {
                DocumentId = dto.DocumentId,
                Name = dto.Name,
                Balance = dto.Balance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return wallet.Id;
        }

        public async Task<bool> UpdateWalletAsync(int id, WalletDto dto)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
                return false;

            wallet.Name = dto.Name;
            wallet.UpdatedAt = DateTime.UtcNow;

            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWalletAsync(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
                return false;

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Success, string Message)> TransferAsync(TransferDto dto)
        {
            if (dto.Amount <= 0)
                return (false, "El monto debe ser mayor a cero.");

            var fromWallet = await _context.Wallets.FindAsync(dto.FromWalletId);
            var toWallet = await _context.Wallets.FindAsync(dto.ToWalletId);

            if (fromWallet == null || toWallet == null)
                return (false, "Una o ambas billeteras no existen.");

            if (fromWallet.Balance < dto.Amount)
                return (false, "Saldo insuficiente.");

            fromWallet.Balance -= dto.Amount;
            fromWallet.UpdatedAt = DateTime.UtcNow;

            toWallet.Balance += dto.Amount;
            toWallet.UpdatedAt = DateTime.UtcNow;

            _context.HistoryMovements.AddRange(new[]
            {
                new HistoryMovement
                {
                    WalletId = fromWallet.Id,
                    Amount = dto.Amount,
                    Type = "Débito",
                    CreatedAt = DateTime.UtcNow
                },
                new HistoryMovement
                {
                    WalletId = toWallet.Id,
                    Amount = dto.Amount,
                    Type = "Crédito",
                    CreatedAt = DateTime.UtcNow
                }
            });

            await _context.SaveChangesAsync();
            return (true, "Transferencia completada exitosamente.");
        }
    }
}
