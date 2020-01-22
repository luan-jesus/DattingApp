using Microsoft.AspNetCore.Mvc;
using DattingApp.API.Data;
using System.Threading.Tasks;
using DattingApp.API.Models;

namespace DattingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            username = username.ToLower();

            if (await _repo.UserExists(username))
                return BadRequest("Username already exists!");

            var UserToCreate = new User
            {
                Username = username
            }

            var createdUser = await _repo.Register(UserToCreate, password);

            return StatusCode(201);
        }
    }
}