using FilmFreakApi.Application.Models;

public class ImportItemBuilder
{
    private ImportItem _importItem;

    public ImportItemBuilder()
    {
        _importItem = new ImportItem("", "", "", "", "", "", "");
    }

    public ImportItemBuilder WithExternalId(string externalId)
    {
        _importItem = _importItem with { ExternalId = externalId };
        return this;
    }

    public ImportItem Build()
    {
        return _importItem;
    }
}