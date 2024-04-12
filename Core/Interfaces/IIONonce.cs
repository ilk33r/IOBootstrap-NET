using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIONonce<TViewModel, TDBContext> : IIOController<TViewModel, TDBContext>
where TDBContext : IODatabaseContext<TDBContext>
where TViewModel : IIOViewModel<TDBContext>, new()
{

}
