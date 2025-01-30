using Microsoft.AspNetCore.Mvc;
using BLL.Services;
using DTO;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend2024ExampleApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUsersService _usersService;
        private readonly ITokenService _tokenService;
        public UserController(IUsersService usersService, ITokenService tokenService, ILogger<UserController> logger) { 
          _usersService = usersService;
            _tokenService = tokenService;
            _logger = logger;
        }
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register new user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(UserRegisterModelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> Register([FromBody] UserRegisterModelDto model)
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
        [SwaggerOperation(Summary = "Log in to the system.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(UserRegisterModelDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
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

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "Refresh token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(RefreshCredentialsDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> Refresh([FromBody] RefreshCredentialsDto model)
        {
            try
            {
                return Ok(await _usersService.Refresh(model.RefreshToken));
            }
            catch (SecurityTokenException  ex)
            {
                return Unauthorized("Token is expired!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound("User is not found!");
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Log out system user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (User == null || User.Claims == null)
                {
                    _logger.LogWarning("User or User.Claims is null.");
                    return BadRequest("User not authenticated.");
                }

                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "Id");
                if (userIdClaim == null)
                {
                    _logger.LogWarning("Logout attempted without a valid user ID claim.");
                    return BadRequest("User ID claim not found.");
                }

                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                await _tokenService.Logout(Guid.Parse(userIdClaim.Value), token);
                _logger.LogInformation($"User {userIdClaim.Value} successfully logged out.");
                return Ok(new { Message = "Success" });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid user ID format in Logout endpoint.");
                return BadRequest("Invalid user ID format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while logging out.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpGet("Profile")]
        [SwaggerOperation(Summary = "Get User Profile.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.", typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> Profile()
        {
            var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
            return Ok(await _usersService.GetProfile(emailClaim.Value));
        }

        [Authorize]
        [HttpPut("Profile")]
        [SwaggerOperation(Summary = "Edit User Profile.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "InternalServerError.", typeof(Response))]
        public async Task<IActionResult> EditProfile([FromBody] UserEditModelDto model)
        {
            try
            {
                var emailClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                await _usersService.UpdateProfile(emailClaim.Value, model);
                return Ok("Profile updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating profile");
                return Problem();
            }
        }

    }
}

