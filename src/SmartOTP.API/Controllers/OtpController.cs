using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOTP.Application.DTOs;
using SmartOTP.Application.Features.Otp.Commands;
using SmartOTP.Application.Features.Otp.Queries;

namespace SmartOTP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OtpController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet("generate/{accountId}")]
    public async Task<ActionResult<OtpCodeDto>> GenerateOtp(Guid accountId)
    {
        try
        {
            var userId = GetUserId();
            var query = new GenerateOtpQuery
            {
                UserId = userId,
                AccountId = accountId
            };
            var result = await mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("verify")]
    public async Task<ActionResult<bool>> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        try
        {
            command.UserId = GetUserId();
            var result = await mediator.Send(command);
            return Ok(new { isValid = result });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
