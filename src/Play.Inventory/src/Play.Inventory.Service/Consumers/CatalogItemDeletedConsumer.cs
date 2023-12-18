namespace Play.Inventory.Service.Consumers
{
    using System.Threading.Tasks;
    using MassTransit;

    using Play.Common;
    using Play.Inventory.Service.Entities;
    using static Play.Catalog.Contracts.Contracts;

    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> repository;

        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            this.repository = repository;
        }
        
        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await this.repository.GetAsync(message.ItemId);

            if (item == null)
            {
                return;
            }

            await this.repository.RemoveAsync(message.ItemId);
        }
    }
}