using GymBuddy.Api.Common.Interfaces;
using System.Security.Claims;

namespace GymBuddy.Api.Common.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}