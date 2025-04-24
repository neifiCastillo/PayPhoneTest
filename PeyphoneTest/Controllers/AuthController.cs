using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PeyphoneTest.Custom;
using PeyphoneTest.Models;
using PeyphoneTest.Models.Dtos;

namespace PeyphoneTest.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DbpayphoneTestContext _DbpayphoneTestContext;
        private readonly Utilities _utilities;

        public AuthController(DbpayphoneTestContext dbpayphoneTestContext, Utilities utilities)
        {
            _DbpayphoneTestContext = dbpayphoneTestContext;
            _utilities = utilities;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserDto userObj)
        {
            var modelUser = new User
            {
                Name = userObj.Name,
                Email = userObj.Email,
                Password = _utilities.encryptSHA256(userObj.Password)

            };

            await _DbpayphoneTestContext.AddAsync(modelUser);
            await _DbpayphoneTestContext.SaveChangesAsync();

            if (modelUser.IdUser != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto loginObj)
        {
            var findUser = await _DbpayphoneTestContext.Users.Where(u => u.Email == loginObj.Email && u.Password == _utilities.encryptSHA256(loginObj.Password)).FirstOrDefaultAsync();

            if(findUser == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true , token = _utilities.generateJWT(findUser)});

        }

    }
}
