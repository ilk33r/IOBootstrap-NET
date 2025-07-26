using IOBootstrap.NET.Common.Models.Users;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOBackOfficeViewModel<TDBContext> : IIOViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
{
    #region Publics

    public IOUserInfoModel? UserModel { get; set; }

    #endregion
        
    #region View Model Methods
    
    public bool IsBackOffice();

    #endregion
}
