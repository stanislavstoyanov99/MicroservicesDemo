namespace Play.Inventory.Service.Entities
{
    using Play.Common;

    public class CatalogItem : IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}