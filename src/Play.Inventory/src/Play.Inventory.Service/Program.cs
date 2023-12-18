using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;

using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryitems")
    .AddMongoRepository<CatalogItem>("catalogitems")
    .AddMassTransitWithRabbitMq();

// AddCatalogClient(builder.Services);

builder.Services
    .AddControllers(options =>
    {
        options.SuppressAsyncSuffixInActionNames = false;
    });

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void AddCatalogClient(IServiceCollection services)
{
    var jitterer = new Random();

    services
        .AddHttpClient<CatalogClient>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:5001");
        })
        // Retries with exponential backoff
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000))
        ))
        .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
            3,
            TimeSpan.FromSeconds(15)
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}