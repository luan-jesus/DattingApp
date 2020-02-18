using Microsoft.AspNetCore.Mvc;
using DattingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using AutoMapper;
using DattingApp.API.DTOs;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using DattingApp.API.Models;

namespace DattingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDattingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDattingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserForDetailDTO>(user);
            
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDTO UserForUpdateDTO) 
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(UserForUpdateDTO, userFromRepo);

            if (await _repo.SaveAll())
                return NoContent();

                throw new System.Exception($"Updating user with {id} failed on save");
        }
    }
}