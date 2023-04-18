
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
        DoImportAsync(ImportItem[] importItems, string userId)
    {
        var newItems = new List<Release>();
        var updatedItems = new List<Release>();
        var updatedIds = new List<string>();
        var addedIds = new List<string>();
        var externalIds = await _repository.GetExternalIdsAsync(userId);
        // set the same modification time for the whole batch
        var modificationTime = DateTime.UtcNow;
        foreach (var importItem in importItems)
        {
            if (externalIds.Contains(importItem.ExternalId))
            {
                var itemInDb = await _repository.GetByExternalId(importItem.ExternalId, userId);
                if (itemInDb == null) throw new Exception($"Got null with external id {importItem.ExternalId}.");
                if (itemInDb.IsShared == false)
                {
                    _logger.LogInformation("Updating release with externalId {externalId}", importItem.ExternalId);
                    // update release only when it's not shared
                    itemInDb.Barcode = importItem.Barcode;
                    itemInDb.Name = importItem.LocalName;
                    itemInDb.ModifiedTime = modificationTime;
                }
                else
                {
                    _logger.LogWarning("Release has IsShared-status and will not be updated.");
                }

                // update also collection item created by import
                var collectionItem = itemInDb.CollectionItems
                    .SingleOrDefault(ci => ci.ExternalId == importItem.ExternalId && ci.UserId == userId);

                if (collectionItem != null)
                {
                    _logger.LogInformation("Updating collection item with externalId {externalId}", importItem.ExternalId);
                    // TODO update collection item properties 
                    collectionItem.Condition = Condition.Unknown;
                    collectionItem.CollectionStatus = CollectionStatus.Unknown;
                    collectionItem.ModifiedTime = modificationTime;
                }

                updatedItems.Add(itemInDb);
                updatedIds.Add(importItem.ExternalId);
            }
            else
            {
                _logger.LogInformation("Adding release with externalId {externalId}", importItem.ExternalId);
                var release = new Release
                {
                    Barcode = importItem.Barcode,
                    ExternalId = importItem.ExternalId,
                    Name = importItem.LocalName,
                    CreatedTime = modificationTime,
                    UserId = userId,
                    IsShared = false,
                };

                release.CollectionItems.Add(new CollectionItem
                {
                    CreatedTime = modificationTime,
                    // TODO set fields from import model
                    CollectionStatus = CollectionStatus.Unknown,
                    Condition = Condition.Unknown,
                    UserId = userId,
                });

                newItems.Add(release);
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