using MediatR;
using Nursing_Home.Application.Common.Security;
using System.ComponentModel;

namespace Nursing_Home.Application.Features.Accounts.Commands;
public sealed record LoginCommand : IRequest<AccessTokenResponse>
{
    [DefaultValue("user")]
    public string Username { get; init; } = default!;

    [DefaultValue("1")]
    public string Password { get; init; } = default!;

}