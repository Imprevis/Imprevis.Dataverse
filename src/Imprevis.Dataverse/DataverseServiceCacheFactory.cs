namespace Imprevis.Dataverse;

using Imprevis.Dataverse.Abstractions;
using Microsoft.Extensions.Caching.Hybrid;
using System;

internal class DataverseServiceCacheFactory(HybridCache hybridCache) : IDataverseServiceCacheFactory
{
    public IDataverseServiceCache Create(Guid organizationId)
    {
        return new DataverseServiceCache(hybridCache, organizationId);
    }
}
