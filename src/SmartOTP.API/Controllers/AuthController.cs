using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOTP.Application.DTOs;
using SmartOTP.Application.Features.Auth.Commands;

namespace SmartOTP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand command)
    {
        try
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
    {
        try
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        try
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
