using IOBootstrap.NET.Common.Messages.Configuration;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.BackOffice.Configuration.Interfaces;

public interface IIOBackOfficeConfigurationsViewModel : IIOBackOfficeViewModel
{
    public void AddConfigItem(IOConfigurationAddRequestModel requestModel);

    public void DeleteConfigItem(int configurationId);

    public IList<IOConfigurationModel> GetConfigurations();

    public void UpdateConfigItem(IOConfigurationUpdateRequestModel requestModel);
}
