using System;
using System.Collections.Generic;
using System.Text;

namespace ServerApp.Shared.Abstractions.Queries
{
    public interface IQuery
    {
    }

    public interface IQuery<TResult> : IQuery
    {
    }
}