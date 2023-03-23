using FilmFreakApi.Models;

namespace FilmFreakApi.Services;

public interface IImportService
{
    Task<(List<string> addedItems, List<string> updatedItems)>
        DoImportAsync(ImportItem[] importItems);
}

public class ImportService : IImportService
{
    private readonly FilmFreakContext _dbContext;
    private readonly ILogger<ImportService> _logger;

    public ImportService(FilmFreakContext dbContext, ILogger<ImportService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<(List<string> addedItems, List<string> updatedItems)>
        DoImportAsync(ImportItem[] importItems)
    {
        var newItems = new List<Release>();
        var updatedIds = new List<string>();
        var addedIds = new List<string>();
        foreach (var importItem in importItems)
        {
            var itemInDb = _dbContext.Releases
                .FirstOrDefault(r => r.ExternalId == importItem.ExternalId);
            if (itemInDb != null)
            {
                _logger.LogInformation("Updating release with externalId {externalId}", importItem.ExternalId);
                itemInDb.Barcode = importItem.Barcode;
                itemInDb.Title = importItem.LocalName;
                updatedIds.Add(importItem.ExternalId);
            }
            else
            {
                _logger.LogInformation("Adding release with externalId {externalId}", importItem.ExternalId);
                newItems.Add(new Release
                {
                    Barcode = importItem.Barcode,
                    ExternalId = importItem.ExternalId,
                    Title = importItem.LocalName
                });
                addedIds.Add(importItem.ExternalId);
            }
        }
        if (newItems.Any())
        {
            await _dbContext.Releases.AddRangeAsync(newItems);
        }
        await _dbContext.SaveChangesAsync();
        return (addedItems: addedIds, updatedItems: updatedIds);
    }
}