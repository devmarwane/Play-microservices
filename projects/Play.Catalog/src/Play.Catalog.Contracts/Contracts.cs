namespace Play.Catalog.Contracts
{
    public record CatalogItelCreated(Guid ItemId, string Name, string Description);

    public record CatalogItelUpdated(Guid ItemId, string Name, string Description);

    public record CatalogItelDeleted(Guid ItemId);
}
