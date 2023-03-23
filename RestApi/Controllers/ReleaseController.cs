using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmFreakApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace FilmFreakApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReleaseController : ControllerBase 
{
    private readonly FilmFreakContext _context;

    public ReleaseController(FilmFreakContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ReleaseDTO>> PostRelease(Release release)
    {
        _context.Releases.Add(release);
        await _context.SaveChangesAsync();
        // Returns 201 with location header and todo-item in response body 
        return CreatedAtAction(nameof(GetRelease), new { id = release.Id }, ReleaseToDTO(release));
    }

    [HttpGet]
    public async Task<IEnumerable<ReleaseDTO>> GetReleases()
    {
        var releases = await _context.Releases.ToListAsync();
        var result = releases.Select(i => ReleaseToDTO(i));
        return result;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReleaseDTO>> GetRelease(long id)
    {
        var release = await _context.Releases.FindAsync(id);
        if (release == null) return NotFound();
        return ReleaseToDTO(release);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoItem(long id, Release release)
    {
        if (id != release.Id)
        {
            return BadRequest();
        }
        if (!_context.Releases.Any(i => i.Id == id))
        {
            return NotFound();
        }
    
        _context.Entry(release).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        var release = await _context.Releases.FindAsync(id);
        if (release == null)
        {
            return NotFound();
        }
        _context.Releases.Remove(release);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private ReleaseDTO ReleaseToDTO(Release release) => new ReleaseDTO
    {
        Id = release.Id,
        Title = release.Title,
        Barcode = release.Barcode
    };
}