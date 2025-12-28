using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIORateLimit<TViewModel, TDBContext> : IIOController<TViewModel, TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
where TViewModel : IIOViewModel<TDBContext>, new()
{
    public HttpContext HttpContext { get; }
}
