using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nursing_Home.Application.Common.Models;
using Nursing_Home.Application.Common.Security;
using Nursing_Home.Application.Features.Accounts.Commands;

namespace Nursing_Home.WebApi.Controllers;
[Route("api/auths")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ISender sender;

    public AccountController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenResponse>> Login(LoginCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(request, cancellationToken);
    }
    [HttpPost("change-password")]
    public async Task<ActionResult<OperationResult<string>>> ChangePassword(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await sender.Send(request, cancellationToken);
    }

    [HttpPost("register")]
    public async Task<ActionResult<OperationResult<string>>> CreateUser(RegisterCommand command, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }
}
