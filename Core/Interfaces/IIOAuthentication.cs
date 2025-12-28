using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOAuthentication<TDBContext> : IIOUserCredential<TDBContext> 
where TDBContext : IOBaseDatabaseContext<TDBContext>
{

}
