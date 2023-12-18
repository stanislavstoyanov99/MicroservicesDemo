namespace Play.Common.MassTransit
{
    using System.Reflection;
    using global::MassTransit;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Play.Common.Settings;
    
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services
                .AddMassTransit(configure =>
                {
                    configure.AddConsumers(Assembly.GetEntryAssembly());
                    
                    configure.UsingRabbitMq((context, configurator) =>
                    {
                        var configuration = context.GetService<IConfiguration>();
                        var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                        var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                        configurator.Host(rabbitMQSettings.Host);
                        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    });
                });
            
            return services;
        }
    }
}