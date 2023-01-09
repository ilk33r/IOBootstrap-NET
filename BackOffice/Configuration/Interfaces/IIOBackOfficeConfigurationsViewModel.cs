using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.BackOffice.Configuration.Interfaces;

public interface IIOBackOfficeConfigurationsViewModel<TDBContext> : IIOBackOfficeViewModel<TDBContext>
where TDBContext : IODatabaseContext<TDBContext> 
{
    public void AddConfigItem(IOConfigurationAddRequestModel requestModel);

    public void DeleteConfigItem(int configurationId);

    public IList<IOConfigurationModel> GetConfigurations();

    public void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel);
}
