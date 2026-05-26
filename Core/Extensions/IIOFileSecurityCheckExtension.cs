using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIOFileSecurityCheckExtension
{
    private class MagicByteValidator
    {
        byte[][]? StartsWith;

        byte[][]? Anywhere;

        public MagicByteValidator(byte[][]? startsWith = null, byte[][]? anywhere = null)
        {
            this.StartsWith = startsWith;
            this.Anywhere = anywhere;
        }

        public byte[]? Validate(Stream fileStream)
        {
            if (StartsWith != null)
            {
                int byteLength = StartsWith[0].Length;
                byte[] fileBytes = new byte[byteLength];
                _ = fileStream.Read(fileBytes, 0, byteLength);
                byte[]? outputMagicBytes = null;

                foreach (byte[] expectedBytes in StartsWith)
                {
                    outputMagicBytes = new byte[byteLength];

                    for (int i = 0; i < expectedBytes.Length; i++)
                    {
                        if (fileBytes[i] == expectedBytes[i])
                        {
                            outputMagicBytes[i] = expectedBytes[i];
                        }
                        else
                        {
                            outputMagicBytes = null;
                            break;
                        }
                    }

                    if ((outputMagicBytes?.Count() ?? 0) == expectedBytes.Length)
                    {
                        break;
                    }
                    else
                    {
                        outputMagicBytes = null;
                    }
                }

                if (outputMagicBytes == null)
                {
                    return null;
                }

                return outputMagicBytes;
            }

            if (Anywhere != null)
            {
                byte[] fileBytes = new byte[256];
                _ = fileStream.Read(fileBytes, 0, 256);
                bool isValid;

                foreach (byte[] expectedBytes in Anywhere)
                {
                    for (int i = 0; i < fileBytes.Length; i++)
                    {
                        isValid = true;
                        for (int j = 0; j < expectedBytes.Length; j++)
                        {
                            if (fileBytes[i + j] != expectedBytes[j])
                            {
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid)
                        {
                            return fileBytes;
                        }
                    }
                }
            }

            return null;
        }
    }

    public static (string, byte[], Stream) CheckFile(this IIOFileSecurityCheck input, IFormFile file)
    {
        long maxFileSize = input.Configuration.GetValue<long>(IOConfigurationConstants.IOMaxUploadFileSize);
        if (file.Length > maxFileSize)
        {
            throw new IOImageFileSizeException();
        }

        MagicByteValidator? magicByteValidator = null;
        string newFileName = "";

        if (file.ContentType.ToLower().Equals("image/jpg") || file.ContentType.ToLower().Equals("image/jpe") || file.ContentType.ToLower().Equals("image/jpeg") || file.ContentType.ToLower().Equals("image/pjpeg"))
        {
            if (!file.FileName.ToLower().EndsWith(".jpg") && !file.FileName.ToLower().EndsWith(".jpe") && !file.FileName.ToLower().EndsWith(".jpeg") && !file.FileName.ToLower().EndsWith(".pjpeg") && !file.FileName.ToLower().EndsWith(".jfif") && !file.FileName.ToLower().EndsWith(".pjp"))
            {
                throw new IOImageCorruptException("File " + file.ContentType + " is not expected content.");
            }

            magicByteValidator = new MagicByteValidator(
                startsWith: [[0xFF, 0xD8, 0xFF]]
            );
            
            newFileName = String.Format("{0}-{1}.jpg", IORandomUtilities.GenerateGUIDString(), file.FileName.Substring(0, file.FileName.Count() - 4).RemoveNonASCII());
        }

        if (file.ContentType.ToLower().Equals("image/png"))
        {
            if (!file.FileName.ToLower().EndsWith(".png"))
            {
                throw new IOImageCorruptException("File " + file.ContentType + " is not expected content.");
            }

            magicByteValidator = new MagicByteValidator(
                startsWith: [[0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]]
            );

            newFileName = String.Format("{0}-{1}.png", IORandomUtilities.GenerateGUIDString(), file.FileName.Substring(0, file.FileName.Count() - 4).RemoveNonASCII());
        }

        if (file.ContentType.ToLower().Equals("image/heic"))
        {
            if (!file.FileName.ToLower().EndsWith(".heic"))
            {
                throw new IOImageCorruptException("File " + file.ContentType + " is not expected content.");
            }

            magicByteValidator = new MagicByteValidator(
                anywhere: [[0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63]]
            );

            newFileName = String.Format("{0}-{1}.heic", IORandomUtilities.GenerateGUIDString(), file.FileName.Substring(0, file.FileName.Count() - 5).RemoveNonASCII());
        }

        if (file.ContentType.ToLower().Equals("video/h263") || file.ContentType.ToLower().Equals("video/h264") || file.ContentType.ToLower().Equals("video/mpeg"))
        {
            if (!file.FileName.ToLower().EndsWith(".mpeg") && !file.FileName.ToLower().EndsWith(".mpg"))
            {
                throw new IOImageCorruptException("File " + file.ContentType + " is not expected content.");
            }

            magicByteValidator = new MagicByteValidator(
                startsWith: [
                    [0x00, 0x00, 0x01, 0xB3],
                    [0x00, 0x00, 0x01, 0xBA]
                ]
            );

            newFileName = String.Format("{0}-{1}.mpeg", IORandomUtilities.GenerateGUIDString(), file.FileName.Substring(0, file.FileName.Count() - 4).RemoveNonASCII());
        }

        if (file.ContentType.ToLower().Equals("video/mp4") || file.ContentType.ToLower().Equals("video/quicktime"))
        {
            if (!file.FileName.ToLower().EndsWith(".mp4") && !file.FileName.ToLower().EndsWith(".mov"))
            {
                throw new IOImageCorruptException("File " + file.ContentType + " is not expected content.");
            }

            magicByteValidator = new MagicByteValidator(
                anywhere: [
                    [0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D],
                    [0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32],
                    [0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x3E, 0x56],
                    [0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56],
                    [0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20]
                ]
            );

            newFileName = String.Format("{0}-{1}.mp4", IORandomUtilities.GenerateGUIDString(), file.FileName.Substring(0, file.FileName.Count() - 4).RemoveNonASCII());
        }

        if (magicByteValidator == null)
        {
            throw new IOImageCorruptException("File Content-Type " + file.ContentType + " is not supported.");
        }

        Stream fileStream = file.OpenReadStream();
        byte[]? magicBytes = magicByteValidator.Validate(fileStream);

        if (magicBytes == null)
        {
            fileStream.Close();
            fileStream.Dispose();
            throw new IOImageCorruptException("File " + file.ContentType + " is not expected binary.");
        }

        return (newFileName, magicBytes, fileStream);
    }
}
