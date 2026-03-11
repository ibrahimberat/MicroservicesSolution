using AuthService.Application.Commands;
using AuthService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var command = new LoginCommand { LoginDto = loginDto };
                var result = await _mediator.Send(command);
                _logger.LogInformation("User logged in successfully");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Login failed");
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var command = new RegisterCommand { RegisterDto = registerDto };
                var result = await _mediator.Send(command);
                _logger.LogInformation("User registered successfully: {Email}", registerDto.Email);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var command = new RefreshTokenCommand { RefreshToken = refreshTokenDto.RefreshToken };
                var result = await _mediator.Send(command);
                _logger.LogInformation("Token refreshed successfully");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Token refresh failed");
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var command = new RevokeTokenCommand { RefreshToken = refreshTokenDto.RefreshToken };
                var result = await _mediator.Send(command);

                if (result)
                {
                    _logger.LogInformation("Token revoked successfully");
                    return Ok(new { message = "Token revoked successfully" });
                }

                return BadRequest(new { message = "Failed to revoke token" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token revocation failed");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            // Profile information would be returned here
            return Ok(new
            {
                UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value,
                Username = User.Identity?.Name
            });
        }
    }
}