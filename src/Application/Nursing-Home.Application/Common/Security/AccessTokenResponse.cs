using Nursing_Home.Application.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nursing_Home.Application.Common.Security;
public sealed record AccessTokenResponse
{
    public string TokenType { get; } = Tokens.Bearer;

    public required string AccessToken { get; init; }

    public required long ExpiresIn { get; init; }

    public required string RefreshToken { get; init; }
}

