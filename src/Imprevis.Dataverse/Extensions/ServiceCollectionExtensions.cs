namespace Imprevis.Dataverse.Extensions;

using Imprevis.Dataverse.Abstractions;
using Imprevis.Dataverse.Exceptions;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IDataverseServiceBuilder AddDataverseServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddLogging();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        services.AddOptions<DataverseServiceFactoryOptions>()
            .BindConfiguration(DataverseServiceFactoryOptions.Section)
            .ValidateOnStart();

        services.AddSingleton<IDataverseServiceCacheFactory, DataverseServiceCacheFactory>();
        services.AddSingleton<IDataverseServiceFactory, DataverseServiceFactory>();

        services.AddTransient(provider =>
        {
            var factory = provider.GetRequiredService<IDataverseServiceFactory>();
            var resolvers = provider.GetServices<IDataverseServiceResolver>();

            if (factory.GetServices().Count() == 1 && !resolvers.Any())
            {
                return factory.GetServices().First();
            }

            if (factory.GetServices().Count() > 1 && !resolvers.Any())
            {
                throw new DataverseServiceResolverMissingException();
            }

            foreach (var resolver in resolvers)
            {
                var organizationId = resolver.Resolve();
                if (organizationId.HasValue)
                {
                    return factory.GetService(x => x.OrganizationId == organizationId.Value);
                }
            }

            throw new DataverseServiceNotResolvedException();
        });

        return new DataverseServiceBuilder(services);
    }

    public static IDataverseServiceBuilder AddDataverseServices(this IServiceCollection services, Action<DataverseServiceFactoryOptions> configureOptions)
    {
        var builder = services.AddDataverseServices();
        services.Configure(configureOptions);

        return builder;
    }
}
