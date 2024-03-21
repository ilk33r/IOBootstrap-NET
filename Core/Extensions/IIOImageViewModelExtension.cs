using System;
using System.IO;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOImageViewModelExtension
{

    public static string SaveFile(this IIOImageViewModel input, IFormFile file)
    {
        if (file.Length < 16)
        {
            throw new IOImageCorruptException();
        }

        byte[] jpegImage;
        try {
            Image rawImage = Image.Load(file.OpenReadStream());
            jpegImage = ResizedAndEncodeImage(rawImage);
        }
        catch (Exception e)
        {
            input.Logger.LogDebug("{0}", e.StackTrace);
            throw new IOImageCorruptException();
        }

        if (jpegImage == null)
        {
            throw new IOImageCorruptException();
        }

        string imagesFolder = input.Configuration.GetValue<string>(IOConfigurationConstants.ImagesFolderKey);
        string newFileName = String.Format("{0}-{1}.jpg", IORandomUtilities.GenerateGUIDString(), file.FileName.RemoveNonASCII());
        string filePath = Path.Combine(imagesFolder, newFileName);
        
        try
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            fileStream.Write(jpegImage, 0, jpegImage.Length);
            fileStream.Flush();
            return filePath;
        }
        catch
        {
            throw new IOImageSaveException();
        }
    }

    public static void RemoveFile(this IIOImageViewModel input, string fileName)
    {
        string imagesFolder = input.Configuration.GetValue<string>(IOConfigurationConstants.ImagesFolderKey);
        string filePath = Path.Combine(imagesFolder, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
                throw new IOImageNotFoundException();
            }
        }
    }

    private static byte[] ResizedAndEncodeImage(Image image)
    {
        int currentWidth = image.Width;
        int currentHeight = image.Height;

        MemoryStream memoryStream = new MemoryStream();
        if (currentWidth <= 1920 && currentHeight <= 1080)
        {
            image.SaveAsJpeg(memoryStream);
            memoryStream.Flush();

            return memoryStream.ToArray();
        }

        float imageAspectRatio = (float)currentWidth / (float)currentHeight;
        float canvasAspectRatio = (float)1920 / (float)1080;
        float resizeFactor = 0;

        if (imageAspectRatio > canvasAspectRatio) {
            resizeFactor = (float)1920 / currentWidth;
        } else {
            resizeFactor = (float)1080 / currentHeight;
        }

        float scaledWidth = (float)currentWidth * resizeFactor;
        float scaledHeight = (float)currentHeight * resizeFactor;

        image.Mutate(im => im.Resize((int)scaledWidth, (int)scaledHeight, true));

        image.SaveAsJpeg(memoryStream);
        memoryStream.Flush();

        return memoryStream.ToArray();
    }
}