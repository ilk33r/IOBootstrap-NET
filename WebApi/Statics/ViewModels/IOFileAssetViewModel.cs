using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Files;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.WebApi.Statics;

public class IOFileAssetViewModel<TDBContext> : IOViewModel<TDBContext>, IIOFileAssetViewModel
    where TDBContext : IOBaseDatabaseContext<TDBContext>
{

    public override void CheckAuthorizationHeader()
    {
    }

    public (FileStream, string) GetFile(string publicId)
    {
        string? fileName = this.GetFileName(publicId);
        if (String.IsNullOrEmpty(fileName))
        {
            throw new IOFileNotFoundException();
        }

        string filesFolder = Configuration.GetValue<string>(IOConfigurationConstants.FilesFolderKey)!;
        string filePath = Path.Combine(filesFolder, fileName);

        if (!File.Exists(filePath))
        {
            throw new IOFileNotFoundException();
        }

        IOFilesEntity? fileEntity = DatabaseContext.Files
                                                    .Where(e => e.FileName == fileName)
                                                    .FirstOrDefault();
        if (fileEntity == null)
        {
            throw new IOFileNotFoundException();
        }


        return (File.OpenRead(filePath), fileEntity.FileType ?? "application/octet-stream");
    }
}
