using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Custom;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PeyphoneTest.Interfaces;
namespace PeyphoneTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryMovementsController : ControllerBase
    {

        private readonly IHistoryService _historyService;

        public HistoryMovementsController(IHistoryService historyService)
        {
            _historyService = historyService;
        }


        [HttpGet("{walletId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistoryByIdAsync(int walletId)
        {
            try
            {
                var history = await _historyService.GetHistoryByIdAsync(walletId);

                if (history == null || !history.Any())
                    return NotFound(new { success = false, message = "No hay movimientos registrados" });

                return Ok(history);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Error al obtener el historial de la billetera." });
            }
        }

    }
}
