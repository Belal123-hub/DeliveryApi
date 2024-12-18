using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DTO;

namespace Backend2024ExampleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UserController(IUsersService usersService) { 
            this._usersService = usersService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _usersService.Register(model);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("User with same email already exists");
            }
            catch (Exception ex)
            {
                return Problem("Something happened during users registration");
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsDto model)
        {
            try
            {
                return Ok(await _usersService.Login(model));
            }
            catch (ArgumentException ex) 
            {
                return BadRequest("Login or maybe password invalid!");
            }
            catch(Exception ex)
            {
                return Problem();
            }
        }
    }
}

