using Microsoft.AspNetCore.Mvc;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Authorization;
using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Domain.Entities;
using FilmFreakApi.RestApi.Validators;

namespace FilmFreakApi.RestApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReleaseController : ControllerBase
{
    private readonly IReleaseService _releaseService;
    private readonly IFileUploadService _fileUploadService;
    private readonly IFileValidator _fileValidator;

    public ReleaseController(IReleaseService releaseService,
        IFileUploadService fileUploadService)
    {
        _releaseService = releaseService;
        _fileUploadService = fileUploadService;
        _fileValidator = new ImageFileValidator(FileValidationSpecsBuilder.Build(FileUploadType.Jpeg));
    }

    [HttpPost]
    public async Task<ActionResult<ReleaseDTO>> PostRelease(Release release)
    {
        await _releaseService.AddReleaseAsync(release);
        return CreatedAtAction(nameof(GetRelease), new { id = release.Id }, ReleaseToDTO(release));
    }

    [HttpGet]
    public async Task<IEnumerable<ReleaseDTO>> GetReleases()
    {
        var releases = await _releaseService.GetReleasesAsync();
        var result = releases.Select(i => ReleaseToDTO(i));
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReleaseDTO>> GetRelease(long id)
    {
        var release = await _releaseService.GetReleaseAsync(id);
        if (release == null) return NotFound();
        return ReleaseToDTO(release);
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
        _fileValidator.Validate(file);
        var fileId = await _fileUploadService.StoreFile(file.OpenReadStream());

        // TODO:
        // - store file for release in db (fileId) 
        // - after file is stored, trigger a background task 
        //      - to scale it down when needed
        //      - to create and store a thumbnail image to db
        // - return the file id
        return fileId;
    }


    private ReleaseDTO ReleaseToDTO(Release release) => new ReleaseDTO
    {
        Id = release.Id,
        Title = release.Name,
        Barcode = release.Barcode,
        ExternalId = release.ExternalId
    };
}