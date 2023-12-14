namespace Play.Inventory.Service.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Play.Common;
    using Play.Inventory.Service.Clients;
    using Play.Inventory.Service.Dtos;
    using Play.Inventory.Service.Entities;

    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemsRepository;

        private readonly CatalogClient catalogClient;

        public ItemsController(IRepository<InventoryItem> itemsRepository, CatalogClient catalogClient)
        {
            this.itemsRepository = itemsRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await this.catalogClient.GetCatalogItemsAsync();
            var inventoryItems = await this.itemsRepository.GetAllAsync(item => item.UserId == userId);

            var inventoryItemDtos = inventoryItems.Select(inventoryItem => 
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            
            return Ok(inventoryItemDtos);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {
            var inventoryItem = await this.itemsRepository.GetAsync(
                item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await this.itemsRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await this.itemsRepository.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}