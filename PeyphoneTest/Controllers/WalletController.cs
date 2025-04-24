using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;


namespace PeyphoneTest.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly DbpayphoneTestContext _dbpayphoneTestContext;

        public WalletController(DbpayphoneTestContext dbpayphoneTestContext)
        {
            _dbpayphoneTestContext = dbpayphoneTestContext;
        }

        [HttpGet]
        [Route("GetAllWallet")]
        public async  Task<IActionResult> GetAllWallet()
        {
            var list = await _dbpayphoneTestContext.Wallets.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = list });

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetbyIdWallet(int id)
        {
            var wallet = await _dbpayphoneTestContext.Wallets.FindAsync(id);
            if (wallet == null)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            return Ok(wallet);
        }

        [HttpPost]
        [Route("CreateWallet")]
        public async Task<IActionResult> CreateWallet([FromBody] WalletDto walletDto)
        {
            var wallet = new Wallet
            {
                DocumentId = walletDto.DocumentId,
                Name = walletDto.Name,
                Balance = walletDto.Balance,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dbpayphoneTestContext.Wallets.AddAsync(wallet);
            await _dbpayphoneTestContext.SaveChangesAsync();

            return Ok(new { success = true, walletId = wallet.Id });
        }

        [HttpPut]
        [Route("UpdateWallet/{id}")]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] WalletDto walletUpdate)
        {
            var wallet = await _dbpayphoneTestContext.Wallets.FindAsync(id);
            if (wallet == null)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            wallet.Name = walletUpdate.Name;
            wallet.UpdatedAt = DateTime.UtcNow;

            _dbpayphoneTestContext.Wallets.Update(wallet);
            await _dbpayphoneTestContext.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpDelete]
        [Route("DeleteWallet/{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var wallet = await _dbpayphoneTestContext.Wallets.FindAsync(id);
            if (wallet == null)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            _dbpayphoneTestContext.Wallets.Remove(wallet);
            await _dbpayphoneTestContext.SaveChangesAsync();

            return Ok(new { success = true });
        }

       
        [HttpPost]
        [Route("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto transfer)
        {
            if (transfer.Amount <= 0)
                return BadRequest(new { success = false, message = "El monto debe ser mayor a cero." });

            var fromWallet = await _dbpayphoneTestContext.Wallets.FindAsync(transfer.FromWalletId);
            var toWallet = await _dbpayphoneTestContext.Wallets.FindAsync(transfer.ToWalletId);

            if (fromWallet == null || toWallet == null)
                return NotFound(new { success = false, message = "Una o ambas billeteras no existen." });

            if (fromWallet.Balance < transfer.Amount)
                return BadRequest(new { success = false, message = "Saldo insuficiente." });

            fromWallet.Balance -= transfer.Amount;
            fromWallet.UpdatedAt = DateTime.UtcNow;

            toWallet.Balance += transfer.Amount;
            toWallet.UpdatedAt = DateTime.UtcNow;

                _dbpayphoneTestContext.HistoryMovements.AddRange(new[]
                {
                    new HistoryMovement
                    {
                        WalletId = fromWallet.Id,
                        Amount = transfer.Amount,
                        Type = "Débito",
                        CreatedAt = DateTime.UtcNow
                    },
                    new HistoryMovement
                    {
                        WalletId = toWallet.Id,
                        Amount = transfer.Amount,
                        Type = "Crédito",
                        CreatedAt = DateTime.UtcNow
                    }
                });

            await _dbpayphoneTestContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Transferencia completada exitosamente." });
        }


    }
}
