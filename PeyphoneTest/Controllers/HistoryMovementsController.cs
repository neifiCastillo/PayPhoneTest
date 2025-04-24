using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Custom;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace PeyphoneTest.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class HistoryMovementsController : ControllerBase
    {

        private readonly DbpayphoneTestContext _dbpayphoneTestContext;

        public HistoryMovementsController(DbpayphoneTestContext dbpayphoneTestContext)
        {
            _dbpayphoneTestContext = dbpayphoneTestContext;
        }

        [HttpGet]
        [Route("GetHistory")]
        public async Task<IActionResult> GetHistory()
        {
            var list = await _dbpayphoneTestContext.HistoryMovements.ToListAsync();
            return StatusCode(StatusCodes.Status200OK, new { value = list });

        }
    }
}
