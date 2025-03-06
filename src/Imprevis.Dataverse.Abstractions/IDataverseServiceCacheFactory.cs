namespace Imprevis.Dataverse.Abstractions;

public interface IDataverseServiceCacheFactory
{
    IDataverseServiceCache Create(Guid organizationId);
}
