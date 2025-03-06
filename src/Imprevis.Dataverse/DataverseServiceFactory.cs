namespace Imprevis.Dataverse;

using Imprevis.Dataverse.Abstractions;
using Imprevis.Dataverse.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal class DataverseServiceFactory : IDataverseServiceFactory
{
    private readonly ICollection<IDataverseService> services = [];

    public DataverseServiceFactory(IOptions<DataverseServiceFactoryOptions> options, IDataverseServiceCacheFactory cacheFactory, ILoggerFactory loggerFactory)
    {
        foreach (var serviceOptions in options.Value.Services)
        {
            var cache = cacheFactory.Create(serviceOptions.OrganizationId);
            var service = new DataverseService(serviceOptions, cache, loggerFactory);
            services.Add(service);
        }
    }

    /// <summary>
    /// Internal constructor used for unit testing.
    /// </summary>
    internal DataverseServiceFactory(ICollection<IDataverseService> services)
    {
        this.services = services;
    }

    public void Dispose()
    {
        Parallel.ForEach(services, service => service.Dispose());
    }

    public IDataverseService GetService(Func<IDataverseService, bool> predicate)
    {
        var service = GetServices(predicate).FirstOrDefault();
        if (service == null)
        {
            throw new DataverseServiceNotFoundException();
        }

        if (!service.IsReady)
        {
            throw new DataverseServiceNotReadyException();
        }

        return service;
    }

    public IEnumerable<IDataverseService> GetServices(Func<IDataverseService, bool>? predicate = null)
    {
        if (predicate == null)
        {
            return services;
        }

        return services.Where(predicate);
    }
}
