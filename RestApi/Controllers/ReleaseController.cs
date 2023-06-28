using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Domain.Entities;
using FilmFreakApi.RestApi.Validators;
using FilmFreakApi.Application.Models;

namespace FilmFreakApi.RestApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReleaseController : ControllerBase
{
    private readonly IReleaseService _releaseService;
    private readonly IFileUploadService _fileUploadService;
    private readonly ILogger<ReleaseController> _logger;

    public ReleaseController(IReleaseService releaseService,
        IFileUploadService fileUploadService,
        ILogger<ReleaseController> logger)
    {
        _releaseService = releaseService;
        _fileUploadService = fileUploadService;
        _logger = logger;
        _logger.LogInformation("ReleaseController created");
    }

    [HttpPost]
    public async Task<ActionResult<ReleaseDTO>> PostRelease(ReleaseDTO release)
    {
        _logger.LogInformation("PostRelease called");
        await _releaseService.AddReleaseAsync(release);
        return CreatedAtAction(nameof(GetRelease), new { id = release.Id }, release);
    }

    [HttpGet]
    public async Task<IEnumerable<ReleaseDTO>> GetReleases()
    {
        var releases = await _releaseService.GetReleasesAsync();
        var result = releases.Select(i => (ReleaseDTO)i);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReleaseDTO>> GetRelease(long id)
    {
        var release = await _releaseService.GetReleaseAsync(id);
        if (release == null) return NotFound();
        return (ReleaseDTO)release;
    }

    [HttpPut("{id}")]
    public Task<IActionResult> PutRelease(long id, Release release)
    {
        throw new NotImplementedException();
        /*if (id != release.Id)
        {
            return BadRequest();
        }
        if (!_context.Releases.Any(i => i.Id == id))
        {
            return NotFound();
        }

        _context.Entry(release).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();*/
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRelease(long id)
    {
        var release = await _releaseService.GetReleaseAsync(id);
        if (release == null)
        {
            return NotFound();
        }
        await _releaseService.Remove(release);
        return NoContent();
    }

    [HttpPost("{id}/files")]
    public async Task<string> UploadFile(IFormFile file)
    {
        var fileValidator = new ImageFileValidator(new FileValidationSpecs(
            "image/jpeg",
            new List<string> { ".jpg", ".jpeg" },
            // TODO: check if this is the correct signature for a jpeg
            new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 } },
            1000000
        ));
        fileValidator.Validate(file);
        var fileId = await _fileUploadService.StoreFile(file.OpenReadStream());

        // TODO: store file for release in db (fileId) 

        // TODO: return the file id
        return fileId;
    }

}