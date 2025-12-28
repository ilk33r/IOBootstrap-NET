using System;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOFunctionsViewModel<TDBContext> : IIOViewModel<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    public string EncryptResult(string json);
}
