using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.WebApi.Statics;

public class IOImageAssetViewModel<TDBContext> : IOViewModel<TDBContext>, IIOImageAssetViewModel
    where TDBContext : IODatabaseContext<TDBContext> 
{

    public override void CheckAuthorizationHeader()
    {
    }

    public FileStream GetImageFile(string publicId)
    {
        string fileName = this.GetImageFileName(publicId);
        if (String.IsNullOrEmpty(fileName))
        {
            throw new IOImageNotFoundException();
        }

        string imagesFolder = Configuration.GetValue<string>(IOConfigurationConstants.ImagesFolderKey);
        string imagePath = Path.Combine(imagesFolder, fileName);

        if (!File.Exists(imagePath))
        {
            throw new IOImageNotFoundException();
        }

        return File.OpenRead(imagePath);
    }
}
