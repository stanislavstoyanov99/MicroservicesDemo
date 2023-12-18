namespace Play.Inventory.Service.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;

    using Play.Common;
    using Play.Inventory.Service.Entities;
    using static Play.Catalog.Contracts.Contracts;

    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item = await this.repository.GetAsync(message.ItemId);

            if (item != null)
            {
                return;
            }

            item = new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await this.repository.CreateAsync(item);
        }
    }
}