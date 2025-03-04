﻿namespace Imprevis.Dataverse.Resolvers.Http;

using Imprevis.Dataverse.Abstractions;
using Microsoft.AspNetCore.Http;

public class ResolveByRouteValue(IHttpContextAccessor? httpContextAccessor, string name, Func<object?, Guid?>? parse = null) : IDataverseServiceResolver
{
    public Guid? Resolve()
    {
        if (httpContextAccessor == null)
        {
            return null;
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        var success = httpContext.Request.RouteValues.TryGetValue(name, out var value);
        if (success)
        {
            // Use custom processing method
            if (parse != null)
            {
                return parse(value);
            }

            // Parse as a Guid
            var parsed = Guid.TryParse(value?.ToString(), out var organizationId);
            if (parsed)
            {
                return organizationId;
            }
        }

        return null;
    }
}
