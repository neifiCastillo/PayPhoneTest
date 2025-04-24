using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;
using PeyphoneTest.Services;
using PeyphoneTest.Interfaces;
using PeyphoneTest.Models.Responses;
using static PeyphoneTest.Models.Responses.ResponsesModels;


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
            try
            {
                var wallets = await _walletService.GetAllWalletsAsync();
                return Ok(new { value = wallets });
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Error al obtener las billeteras." });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdWallet(int id)
        {
            try
            {
                var wallet = await _walletService.GetWalletByIdAsync(id);
                if (wallet == null)
                    return NotFound(new { success = false, message = "Billetera no encontrada" });

                return Ok(wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Error al obtener la billetera." });
            }
        }


        [HttpPost("CreateWallet")]
        public async Task<IActionResult> CreateWallet([FromBody] WalletDto dto)
        {
            try
            {
                var id = await _walletService.CreateWalletAsync(dto);
                return Ok(new CreateWalletResponse
                {
                    Success = true,
                    WalletId = id
                });
            }
            catch (ArgumentException ex)
            {
    
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al crear la billetera."
                });
            }
        }

        [HttpPut("UpdateWallet/{id}")]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] WalletDto dto)
        {
            try
            {
                var updated = await _walletService.UpdateWalletAsync(id, dto);

                if (!updated)
                    return NotFound(new { success = false, message = "Billetera no encontrada." });

                return Ok(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al actualizar la billetera." });
            }
        }

        [HttpDelete("DeleteWallet/{id}")]
        public async Task<IActionResult> DeleteWallet(int id)
        {
            try
            {
                var deleted = await _walletService.DeleteWalletAsync(id);

                if (!deleted)
                    return NotFound(new { success = false, message = "Billetera no encontrada." });

                return Ok(new { success = true });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar la billetera." });
            }
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferDto dto)
        {
            var result = await _walletService.TransferAsync(dto);

            if (!result.Success)
                return BadRequest(new TransferResponse { Success = false, Message = result.Message });

            return Ok(new TransferResponse { Success = true, Message = result.Message });
        }

    }
}
