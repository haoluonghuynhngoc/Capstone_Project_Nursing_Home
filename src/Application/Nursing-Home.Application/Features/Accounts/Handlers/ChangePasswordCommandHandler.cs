using MediatR;
using Microsoft.AspNetCore.Identity;
using Nursing_Home.Application.Common.Models;
using Nursing_Home.Application.Features.Accounts.Commands;
using Nursing_Home.Application.Repositories;
using Nursing_Home.Domain.Entities.Identities;
using System.Net;

namespace Nursing_Home.Application.Features.Accounts.Handlers;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, OperationResult<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<User> _userRepository;
    private readonly UserManager<User> _userManager;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork,
        UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userRepository = _unitOfWork.Repository<User>();
        _userManager = userManager;
    }

    public async Task<OperationResult<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName)
            ?? throw new HttpRequestException("Authentication required. Please provide valid credentials.", null, HttpStatusCode.Unauthorized);

        if (!await _userManager.CheckPasswordAsync(user, request.OldPassword))
        {
            throw new HttpRequestException("Username already exists.", null, HttpStatusCode.Unauthorized);
        }
        var userChangePassworded = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!userChangePassworded.Succeeded)
        {
            throw new HttpRequestException("Invalid request. Please provide valid data.", null, HttpStatusCode.BadRequest);
        }
        return OperationResult<string>.SuccessResult("Change Password Successfully");

    }
}
