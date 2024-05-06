using MediatR;
using Nursing_Home.Application.Common.Models;

namespace Nursing_Home.Application.Features.Accounts.Commands;
public sealed record RegisterCommand : IRequest<OperationResult<string>>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? Avatar { get; set; }
    public string? FullName { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Email { get; set; }
}
