﻿namespace Imprevis.Dataverse;

using Imprevis.Dataverse.Abstractions;
using Imprevis.Dataverse.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Threading;

internal class DataverseService(DataverseServiceOptions options, ILoggerFactory loggerFactory) : IDataverseService
{
    private ServiceClient? client;

    private ServiceClient Client
    {
        get
        {
            if (client == null || !IsReady)
            {
                throw new DataverseServiceNotReadyException();
            }

            return client;
        }
    }

    public bool IsReady => client?.IsReady ?? false;

    public Guid OrganizationId => options.OrganizationId;
    public string OrganizationName => options.OrganizationName;

    public void Connect()
    {
        try
        {
            var client = new ServiceClient(options.ConnectionString)
            {
                EnableAffinityCookie = false,
            };

            if (client.LastException != null)
            {
                throw client.LastException;
            }

            if (!client.IsReady)
            {
                throw new DataverseServiceNotReadyException();
            }

            if (client.ConnectedOrgId != OrganizationId)
            {
                throw new DataverseServiceConfigurationException("OrganizationId does not match connected service.")
                {
                    Data =
                    {
                        {"OrganizationId", OrganizationId },
                        {"ConnectedOrgId", client.ConnectedOrgId },
                    }
                };
            }

            this.client = client;
        }
        catch (Exception ex)
        {
            // Silently swallow error so creation of service doesn't fail.
            var logger = loggerFactory.CreateLogger<DataverseService>();
            logger.LogError(ex, "An error occurred connecting to Dataverse.");
        }
    }

    public void Dispose()
    {
        client?.Dispose();
    }

    [Obsolete("Use RetrieveAsync instead.", true)]
    public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
    {
        return Task.Run(() => RetrieveAsync(entityName, id, columnSet)).Result;
    }
    public Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet)
    {
        return RetrieveAsync(entityName, id, columnSet, CancellationToken.None);
    }
    public Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet, CancellationToken cancellationToken = default)
    {
        return Client.RetrieveAsync(entityName, id, columnSet, cancellationToken);
    }

    [Obsolete("Use RetrieveMultipleAsync instead.", true)]
    public EntityCollection RetrieveMultiple(QueryBase query)
    {
        return Task.Run(() => RetrieveMultipleAsync(query)).Result;
    }
    public Task<EntityCollection> RetrieveMultipleAsync(QueryBase query)
    {
        return RetrieveMultipleAsync(query, CancellationToken.None);
    }
    public Task<EntityCollection> RetrieveMultipleAsync(QueryBase query, CancellationToken cancellationToken = default)
    {
        return Client.RetrieveMultipleAsync(query, cancellationToken);
    }

    [Obsolete("Use CreateAsync instead.", true)]
    public Guid Create(Entity entity)
    {
        return Task.Run(() => Create(entity)).Result;
    }
    public Task<Guid> CreateAsync(Entity entity)
    {
        return CreateAsync(entity, CancellationToken.None);
    }
    public Task<Guid> CreateAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        return Client.CreateAsync(entity, cancellationToken);
    }
    public Task<Entity> CreateAndReturnAsync(Entity entity, CancellationToken cancellationToken)
    {
        return Client.CreateAndReturnAsync(entity, cancellationToken);
    }

    [Obsolete("Use UpdateAsync instead.", true)]
    public void Update(Entity entity)
    {
        Task.Run(() => Update(entity)).Wait();
    }
    public Task UpdateAsync(Entity entity)
    {
        return UpdateAsync(entity, CancellationToken.None);
    }
    public Task UpdateAsync(Entity entity, CancellationToken cancellationToken = default)
    {
        return Client.UpdateAsync(entity, cancellationToken);
    }

    [Obsolete("Use DeleteAsync instead.", true)]
    public void Delete(string entityName, Guid id)
    {
        Task.Run(() => Delete(entityName, id)).Wait();
    }
    public Task DeleteAsync(string entityName, Guid id)
    {
        return DeleteAsync(entityName, id, CancellationToken.None);
    }
    public Task DeleteAsync(string entityName, Guid id, CancellationToken cancellationToken = default)
    {
        return Client.DeleteAsync(entityName, id, cancellationToken);
    }

    [Obsolete("Use AssociateAsync instead.", true)]
    public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        Task.Run(() => AssociateAsync(entityName, entityId, relationship, relatedEntities)).Wait();
    }
    public Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        return AssociateAsync(entityName, entityId, relationship, relatedEntities, CancellationToken.None);
    }
    public Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities, CancellationToken cancellationToken = default)
    {
        return Client.AssociateAsync(entityName, entityId, relationship, relatedEntities, cancellationToken);
    }

    [Obsolete("Use DisassociateAsync instead.", true)]
    public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        Task.Run(() => Disassociate(entityName, entityId, relationship, relatedEntities)).Wait();
    }
    public Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        return DisassociateAsync(entityName, entityId, relationship, relatedEntities, CancellationToken.None);
    }
    public Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities, CancellationToken cancellationToken = default)
    {
        return Client.DisassociateAsync(entityName, entityId, relationship, relatedEntities, cancellationToken);
    }

    [Obsolete("Use ExecuteAsync instead.", true)]
    public OrganizationResponse Execute(OrganizationRequest request)
    {
        return Task.Run(() => ExecuteAsync(request)).Result;
    }
    public Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request)
    {
        return ExecuteAsync(request, CancellationToken.None);
    }
    public Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request, CancellationToken cancellationToken = default)
    {
        return Client.ExecuteAsync(request, cancellationToken);
    }

    public Task ExecuteAsync(IDataverseRequest request, CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger(request.GetType());
        return request.Execute(this, logger, cancellationToken);
    }
    public Task<TResponse> ExecuteAsync<TResponse>(IDataverseRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var logger = loggerFactory.CreateLogger(request.GetType());
        return request.Execute(this, logger, cancellationToken);
    }
}
