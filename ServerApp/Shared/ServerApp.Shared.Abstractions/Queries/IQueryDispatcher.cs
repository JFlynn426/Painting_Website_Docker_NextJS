using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Queries
{
    public interface IQueryDispatcher
    {
        Task<TResult> QueryAsync<TResult>(IQuery<TResult> query);
    }
}