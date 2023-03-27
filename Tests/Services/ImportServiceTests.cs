using FilmFreakApi.Application.Interfaces;
using FilmFreakApi.Application.Models;
using FilmFreakApi.Application.Services;
using FilmFreakApi.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace FilmFreakApi.Tests.Services;

public class ImportServiceTests
{
    private readonly Mock<IReleaseRepository> _releaseRepositoryMock;
    private readonly Mock<ILogger<ImportService>> _loggerMock;
    private readonly IImportService _ImportService;

    public ImportServiceTests()
    {
        _releaseRepositoryMock = new Mock<IReleaseRepository>();
        _loggerMock = new Mock<ILogger<ImportService>>();
        _ImportService = new ImportService(_releaseRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task DoImportAsync_EmptyInputList_EmptyResultLists()
    {
        var items = new ImportItem[] { };
        var result = await _ImportService.DoImportAsync(items);
        Assert.True(result.addedItems != null);
        Assert.True(result.updatedItems != null);
        Assert.True(result.addedItems.Count() == 0);
        Assert.True(result.updatedItems.Count() == 0);
    }

    [Fact]
    public async Task DoImportAsync_NewImportItem_IdInAddedList()
    {
        var expectedAddId = "123-abc";
        _releaseRepositoryMock
            .Setup((r) => r.GetByExternalId(expectedAddId).Result)
            .Returns((Release?)null);
        var items = new ImportItem[] { new ImportItem(expectedAddId, "", "", "", "", "", "") };
        var result = await _ImportService.DoImportAsync(items);
        Assert.True(result.updatedItems.Count() == 0);
        Assert.True(result.addedItems.Count() == 1);
        Assert.True(result.addedItems.First() == expectedAddId);
    }

    [Fact]
    public async Task DoImportAsync_UpdatedImportItem_IdInUpdatedList()
    {
        var expectedAddId = "123-abc";
        var release = new Release { ExternalId = expectedAddId };
        _releaseRepositoryMock
            .Setup((r) => r.GetByExternalId(expectedAddId).Result)
                .Returns(release);
        var items = new ImportItem[] { new ImportItem(expectedAddId, "", "", "", "", "", "") };
        var result = await _ImportService.DoImportAsync(items);
        Assert.True(result.addedItems.Count() == 0);
        Assert.True(result.updatedItems.Count() == 1);
        Assert.True(result.updatedItems.First() == expectedAddId);
    }
}