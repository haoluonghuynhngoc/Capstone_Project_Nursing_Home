using MediatR;
using Microsoft.AspNetCore.Identity;
using Nursing_Home.Application.Common.Security;
using Nursing_Home.Application.Features.Accounts.Commands;
using Nursing_Home.Application.Services;
using Nursing_Home.Domain.Entities.Identities;
using System.Net;

namespace Nursing_Home.Application.Features.Accounts.Handlers;
public sealed record LoginCommandHandler : IRequestHandler<LoginCommand, AccessTokenResponse>
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<AccessTokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, false, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            throw new HttpRequestException("Your account has been locked due to incorrect input 5 times. Please try again after 5 minutes", null, HttpStatusCode.BadRequest);
        }
        var user = await _userManager.FindByNameAsync(request.Username)
            ?? throw new HttpRequestException("Authentication required. Please provide valid credentials.", null, HttpStatusCode.Unauthorized);
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new HttpRequestException("Authentication required. Please provide valid credentials.", null, HttpStatusCode.Unauthorized);
        }
        return await _jwtService.GenerateTokenAsync(user);
    }
}

