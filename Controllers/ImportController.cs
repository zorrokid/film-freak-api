using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    [HttpPost]
    public Task<IActionResult> ImportFromFile(ImportItem[] importItems)
    {
        throw new NotImplementedException();
    }
}