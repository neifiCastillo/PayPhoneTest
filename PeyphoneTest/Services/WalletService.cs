using PeyphoneTest.Interfaces;
using Microsoft.EntityFrameworkCore;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;
using PeyphoneTest.Validators;

namespace PeyphoneTest.Services
{
    public class WalletService : IWalletService
    {
        private readonly DbpayphoneTestContext _context;

        public WalletService(DbpayphoneTestContext context)
        {
            _context = context;
        }

        public async Task<List<WalletDto>> GetAllWalletsAsync()
        {
            try
            {
                // todo: add mapper as an improvement :D
                return await _context.Wallets
                    .Select(w => new WalletDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Balance = w.Balance,
                        DocumentId = w.DocumentId
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {

                throw new ApplicationException("No se pudo obtener las billeteras.", ex);
            }
        }

        public async Task<Wallet?> GetWalletByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la billetera debe ser mayor a cero.");

            try
            {
                var wallet = await _context.Wallets.FindAsync(id);
                return wallet; 
            }
            catch (Exception ex)
            {

                throw new ApplicationException($"Error al buscar la billetera con ID {id}.", ex);
            }
        }


        public async Task<int> CreateWalletAsync(WalletDto dto)
        {
            var validator = new WalletDtoValidator();
            var validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            try
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
            catch (Exception ex)
            {
                throw new ApplicationException("Error al crear la billetera.", ex);
            }
        }

        public async Task<bool> UpdateWalletAsync(int id, WalletDto dto)
        {
            var validator = new WalletDtoValidator();
            var validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMessage);
            }

            try
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
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al actualizar la billetera con ID {id}.", ex);
            }
        }
        public async Task<bool> DeleteWalletAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID de la billetera debe ser válido.");

            try
            {
                var wallet = await _context.Wallets.FindAsync(id);
                if (wallet == null)
                    return false;

                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al eliminar la billetera con ID {id}.", ex);
            }
        }
        public async Task<(bool Success, string Message)> TransferAsync(TransferDto dto)
        {

            var validator = new TransferDtoValidator();
            var validationResult = validator.Validate(dto);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return (false, errorMessage);
            }

            try
            {
                var fromWallet = await _context.Wallets.FindAsync(dto.FromWalletId);
                var toWallet = await _context.Wallets.FindAsync(dto.ToWalletId);

                if (fromWallet == null || toWallet == null)
                    return (false, "Una o ambas billeteras no existen.");

                if (!string.Equals(toWallet.Name, dto.ToWalletName, StringComparison.OrdinalIgnoreCase))
                    return (false, "El nombre de la cuenta de destino no coincide con el ID proporcionado.");

                if (fromWallet.Balance < dto.Amount)
                    return (false, "Saldo insuficiente.");

                // Actualización de saldos
                fromWallet.Balance -= dto.Amount;
                fromWallet.UpdatedAt = DateTime.UtcNow;

                toWallet.Balance += dto.Amount;
                toWallet.UpdatedAt = DateTime.UtcNow;

                // Registro en historial
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
            catch (Exception)
            {
                return (false, "Ocurrió un error al procesar la transferencia.");
            }
        }


    }
}
