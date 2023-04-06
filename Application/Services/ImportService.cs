
using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Application.Models;
using FilmFreakApi.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FilmFreakApi.Application.Services;


public class ImportService : IImportService
{
    private readonly IReleaseRepository _repository;
    private readonly ILogger<ImportService> _logger;


    public ImportService(IReleaseRepository repository, ILogger<ImportService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<(List<string> addedItems, List<string> updatedItems)>
        DoImportAsync(ImportItem[] importItems)
    {
        var newItems = new List<Release>();
        var updatedItems = new List<Release>();
        var updatedIds = new List<string>();
        var addedIds = new List<string>();
        var externalIds = await _repository.GetExternalIdsAsync();
        foreach (var importItem in importItems)
        {
            if (externalIds.Contains(importItem.ExternalId))
            {
                var itemInDb = await _repository.GetByExternalId(importItem.ExternalId);
                if (itemInDb == null) throw new Exception($"Got null with external id {importItem.ExternalId}.");
                _logger.LogInformation("Updating release with externalId {externalId}", importItem.ExternalId);
                itemInDb.Barcode = importItem.Barcode;
                itemInDb.Name = importItem.LocalName;
                updatedItems.Add(itemInDb);
                updatedIds.Add(importItem.ExternalId);
            }
            else
            {
                _logger.LogInformation("Adding release with externalId {externalId}", importItem.ExternalId);
                newItems.Add(new Release
                {
                    Barcode = importItem.Barcode,
                    ExternalId = importItem.ExternalId,
                    Name = importItem.LocalName
                });
                addedIds.Add(importItem.ExternalId);
            }
        }
        if (newItems.Any())
        {
            await _repository.Add(newItems);
        }
        if (updatedItems.Any())
        {
            await _repository.Update(updatedItems);
        }
        return (addedItems: addedIds, updatedItems: updatedIds);
    }
}