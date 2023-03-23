using FilmFreakApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost]
    public async Task<IActionResult> ImportFromFile(ImportItem[] importItems)
    {
        var result = await _importService.DoImportAsync(importItems);
        return Ok(new
        {
            AddedIds = result.addedItems,
            UpdatedIds = result.updatedItems
        });
    }
}