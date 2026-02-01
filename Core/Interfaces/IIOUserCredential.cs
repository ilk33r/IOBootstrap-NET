using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOUserCredential<TDBContext> : IIOViewModel<TDBContext> 
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    public string[]? TokenExtras { get; set; }
    public IOUserInfoModel? UserModel { get; set; }
}
