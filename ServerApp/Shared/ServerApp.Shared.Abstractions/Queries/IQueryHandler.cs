using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Queries
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : class, IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}