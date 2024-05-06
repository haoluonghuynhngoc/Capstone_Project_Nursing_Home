using MediatR;
using Microsoft.AspNetCore.Identity;
using Nursing_Home.Application.Common.Models;
using Nursing_Home.Application.Features.Accounts.Commands;
using Nursing_Home.Application.Repositories;
using Nursing_Home.Domain.Constants;
using Nursing_Home.Domain.Entities.Identities;
using System.Net;

namespace Nursing_Home.Application.Features.Accounts.Handlers;
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, OperationResult<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<User> _userRepository;
    private readonly UserManager<User> _userManager;
    public RegisterCommandHandler(IUnitOfWork unitOfWork,
       UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userRepository = _unitOfWork.Repository<User>();
        _userManager = userManager;
    }
    public async Task<OperationResult<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Avatar = request.Avatar,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            UserName = request.UserName
        };

        var resultCreateUser = await _userManager.CreateAsync(user, request.Password);
        if (!resultCreateUser.Succeeded)
        {
            throw new HttpRequestException("Username already exists.", null, HttpStatusCode.Conflict);
        }

        resultCreateUser = await _userManager.AddToRolesAsync(user, new[] { RoleName.User });
        if (!resultCreateUser.Succeeded)
        {
            throw new HttpRequestException("Cant Not Add Role To User", null, HttpStatusCode.Conflict);
        }
        return OperationResult<string>.SuccessResult("Create User Successfully");

    }
}
