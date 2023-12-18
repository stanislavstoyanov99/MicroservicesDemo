namespace Play.Inventory.Service.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;

    using Play.Common;
    using Play.Inventory.Service.Entities;
    using static Play.Catalog.Contracts.Contracts;

    public class CatalogItemUpdatedConsumer : IConsumer<CatalogItemUpdated>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemUpdatedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        
        public async Task Consume(ConsumeContext<CatalogItemUpdated> context)
        {
            var message = context.Message;

            var item = await this.repository.GetAsync(message.ItemId);

            if (item == null)
            {
                item = new CatalogItem
                {
                    Id = message.ItemId,
                    Name = message.Name,
                    Description = message.Description
                };

                await this.repository.CreateAsync(item);
            }
            else
            {
                item.Name = message.Name;
                item.Description = message.Description;

                await this.repository.UpdateAsync(item);
            }
        }
    }
}