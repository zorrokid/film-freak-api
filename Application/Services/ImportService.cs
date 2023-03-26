
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
        foreach (var importItem in importItems)
        {
            var itemInDb = await _repository.GetByExternalId(importItem.ExternalId);
            if (itemInDb != null)
            {
                _logger.LogInformation("Updating release with externalId {externalId}", importItem.ExternalId);
                itemInDb.Barcode = importItem.Barcode;
                itemInDb.Title = importItem.LocalName;
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
                    Title = importItem.LocalName
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