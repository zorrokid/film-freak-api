using FilmFreakApi.Domain.Entities;

namespace Tests.Builders;

internal class ReleaseBuilder
{
    private readonly Release _release;
    internal const string DefaultExternalId = "ID-123";
    internal const string DefaultUserId = "uid-123";

    internal ReleaseBuilder()
    {
        _release = new Release()
        {
            UserId = DefaultUserId,
            ExternalId = DefaultExternalId,
            IsShared = false,
            CollectionItems = new List<CollectionItem>
            {
                new CollectionItem
                {
                    ExternalId = DefaultExternalId,
                    UserId = DefaultUserId,
                }
            }
        };
    }

    internal ReleaseBuilder WithUser(string userId)
    {
        _release.UserId = userId;
        foreach (var ci in _release.CollectionItems)
        {
            ci.UserId = userId;
        }
        return this;
    }

    internal ReleaseBuilder WithExternalId(string externalId)
    {
        _release.ExternalId = externalId;
        foreach (var ci in _release.CollectionItems)
        {
            ci.ExternalId = externalId;
        }
        return this;
    }

    internal Release Build()
    {
        return _release;
    }
}