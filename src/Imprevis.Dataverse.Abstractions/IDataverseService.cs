namespace Imprevis.Dataverse.Abstractions;

using Microsoft.PowerPlatform.Dataverse.Client;

public interface IDataverseService : IOrganizationServiceAsync2
{
    bool IsReady { get; }

    Guid OrganizationId { get; }
    string OrganizationName { get; }

    IDataverseServiceCache Cache { get; }

    void Connect();
    void Dispose();

    Task ExecuteAsync(IDataverseRequest request, CancellationToken cancellationToken = default);
    Task<TResponse> ExecuteAsync<TResponse>(IDataverseRequest<TResponse> request, CancellationToken cancellationToken = default);
}
