namespace Imprevis.Dataverse.Abstractions;

public interface IDataverseCachedRequest<TResponse> : IDataverseRequest<TResponse>
{
    string CacheKey { get; }
    TimeSpan CacheDuration { get; }
}
