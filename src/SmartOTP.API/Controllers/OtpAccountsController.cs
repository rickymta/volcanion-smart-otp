using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOTP.Application.DTOs;
using SmartOTP.Application.Features.OtpAccounts.Commands;
using SmartOTP.Application.Features.OtpAccounts.Queries;

namespace SmartOTP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OtpAccountsController(IMediator mediator) : ControllerBase
{
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OtpAccountDto>>> GetUserAccounts()
    {
        var userId = GetUserId();
        var query = new GetUserOtpAccountsQuery { UserId = userId };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<OtpAccountDto>> CreateAccount([FromBody] CreateOtpAccountCommand command)
    {
        try
        {
            command.UserId = GetUserId();
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(GetUserAccounts), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{accountId}")]
    public async Task<ActionResult> DeleteAccount(Guid accountId)
    {
        try
        {
            var userId = GetUserId();
            var command = new DeleteOtpAccountCommand
            {
                UserId = userId,
                AccountId = accountId
            };
            await mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
