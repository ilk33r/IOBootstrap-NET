using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces
{
    public interface IIOFunctionsViewModel<TDBContext> : IIOViewModel<TDBContext> 
    where TDBContext : IODatabaseContext<TDBContext>
    {
        public string EncryptResult(string json);
    }
}
