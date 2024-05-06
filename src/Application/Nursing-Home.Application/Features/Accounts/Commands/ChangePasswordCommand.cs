using MediatR;
using Nursing_Home.Application.Common.Models;

namespace Nursing_Home.Application.Features.Accounts.Commands;
public sealed record ChangePasswordCommand : IRequest<OperationResult<string>>
{
    public string UserName { get; set; } = default!;
    public string OldPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}

