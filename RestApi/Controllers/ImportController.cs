using System.Security.Claims;
using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmFreakApi.RestApi.Controllers;

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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) throw new Exception("userId cannot be null.");
        var result = await _importService.DoImportAsync(importItems, userId);
        return Ok(new
        {
            AddedIds = result.addedItems,
            UpdatedIds = result.updatedItems
        });
    }
}