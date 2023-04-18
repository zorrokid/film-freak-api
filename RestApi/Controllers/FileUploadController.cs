using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.RestApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    public FileUploadController()
    {

    }

    public IActionResult Upload(IFormFile file)
    {
        return Ok();
    }
}