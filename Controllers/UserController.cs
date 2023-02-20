using System.Security.Claims;
using FilmFreakApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public UserModel GetLoggedInUser()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roles = User.FindAll(ClaimTypes.Role);
        if (name == null || id == null || !roles.Any())
        {
            throw new Exception("User data not available.");
        }
        var isAdmin = roles.Any(r => r.Value == UserRoles.Admin);
        return new UserModel(id, name, isAdmin);
    }
}