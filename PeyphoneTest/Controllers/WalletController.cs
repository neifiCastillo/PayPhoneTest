using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;
using PeyphoneTest.Services;
using PeyphoneTest.Interfaces;


namespace PeyphoneTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("GetAllWallet")]
        public async Task<IActionResult> GetAllWallet()
        {
            var wallets = await _walletService.GetAllWalletsAsync();
            return Ok(new { value = wallets });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyIdWallet(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);
            if (wallet == null)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            return Ok(wallet);
        }

        [HttpPost("CreateWallet")]
        public async Task<IActionResult> CreateWallet([FromBody] WalletDto dto)
        {
            var id = await _walletService.CreateWalletAsync(dto);
            return Ok(new { success = true, walletId = id });
        }

        [HttpPut("UpdateWallet/{id}")]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] WalletDto dto)
        {
            var updated = await _walletService.UpdateWalletAsync(id, dto);
            if (!updated)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            return Ok(new { success = true });
        }

        [HttpDelete("DeleteWallet/{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            var deleted = await _walletService.DeleteWalletAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Billetera no encontrada" });

            return Ok(new { success = true });
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var (success, message) = await _walletService.TransferAsync(dto);
            if (!success)
                return BadRequest(new { success = false, message });

            return Ok(new { success = true, message });
        }
    }
}
