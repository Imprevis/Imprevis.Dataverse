namespace Imprevis.Dataverse.Abstractions;

public interface IDataverseServiceCache
{
    Task<T> GetOrCreate<T>(string key, Func<CancellationToken, ValueTask<T>> factory, TimeSpan duration, CancellationToken cancellationToken = default);
    Task Set<T>(string key, T value, TimeSpan duration, CancellationToken cancellationToken = default);
    Task Remove(string key, CancellationToken cancellationToken = default);
    Task Remove(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}
