using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Exceptions.Files;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOFileViewModelExtension
{
    public static string SaveRawFile(this IIOFileViewModel input, IFormFile file)
    {
        if (file == null)
        {
            throw new IOInvalidRequestException("File is required.");
        }

        if (file.Length < 16)
        {
            throw new IOFileCorruptException();
        }

        long maxFileSize = input.Configuration.GetValue<long>(IOConfigurationConstants.IOMaxUploadFileSize);
        if (file.Length > maxFileSize)
        {
            throw new IOFileFileSizeException();
        }

        string filesFolder = input.Configuration.GetValue<string>(IOConfigurationConstants.FilesFolderKey)!;
        string newFileName = String.Format("{0}-{1}", IORandomUtilities.GenerateGUIDString(), file.FileName.RemoveNonASCII());
        string filePath = Path.Combine(filesFolder, newFileName);

        try
        {
            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            using FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            file.CopyTo(fileStream);
            fileStream.Flush();
            return filePath;
        }
        catch (Exception ex)
        {
            string exceptionMessage = $"{ex.Message}\n{ex.StackTrace}";
            input.Logger.LogError(exceptionMessage);
            throw new IOFileSaveException(exceptionMessage);
        }
    }

    public static void RemoveFile(this IIOFileViewModel input, string fileName)
    {
        string filesFolder = input.Configuration.GetValue<string>(IOConfigurationConstants.FilesFolderKey)!;
        string filePath = Path.Combine(filesFolder, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                throw new IOFileNotFoundException();
            }
        }
    }
}
