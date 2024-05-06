using Nursing_Home.Domain.Constants;

namespace Nursing_Home.Application.Common.Constants;
public abstract class Policies
{
    public const string Admin = $"{RoleName.Admin}";
    public const string Staff = $"{RoleName.Staff}";
    public const string User = $"{RoleName.User}";
}