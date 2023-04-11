using FilmFreakApi.Application.Models;

namespace FilmFreakApi.Application.Interfaces;

public interface IImportService
{
    Task<(List<string> addedItems, List<string> updatedItems)>
        DoImportAsync(ImportItem[] importItems, string userId);
}
