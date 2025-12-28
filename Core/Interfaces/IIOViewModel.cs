using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Interfaces;

public interface IIOViewModel<TDBContext> : IIOViewModelBase 
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    #region Properties

    public TDBContext DatabaseContext { get; set; }

    #endregion

    #region Helper Methods

    public void CheckAuthorizationHeader();

    public int GetUserRole();

    #endregion

    #region Encryption Decryption

    public Task<string> DecryptString(string encryptedString);

    public Task<string> EncryptString(string plainString);

    #endregion

    #region Configuration

    public IOConfigurationModel? GetDBConfig(string configKey);

    #endregion
}
