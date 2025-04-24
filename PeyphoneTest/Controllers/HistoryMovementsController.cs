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
    [Authorize]
    [ApiController]
    public class HistoryMovementsController : ControllerBase
    {

        private readonly IHistoryService _historyService;

        public HistoryMovementsController(IHistoryService historyService)
        {
            _historyService = historyService;
        }


        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetHistoryByIdAsync(int walletId)
        {
            var History = await _historyService.GetHistoryByIdAsync(walletId);
            if (History == null)
                return NotFound(new { success = false, message = "No hay movimientos registrados" });

            return Ok(History);
        }
    }
}
