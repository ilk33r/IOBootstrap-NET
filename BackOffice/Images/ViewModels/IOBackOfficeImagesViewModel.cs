using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.DataAccess.Context;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using IOBootstrap.NET.Common.Constants;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using IOBootstrap.NET.DataAccess.Entities;
using System.Linq;

namespace IOBootstrap.NET.BackOffice.Images.ViewModels
{
    public class IOBackOfficeImagesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Initialization Methods

        public IOBackOfficeImagesViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public Tuple<int, IList<IOImageVariationsModel>> GetImages(int start, int count)
        {
            IQueryable<IOImagesEntity> images = DatabaseContext.Images;
            int imageCount = images.Count();
            IList<IOImageVariationsModel> paginatedImages = images.Skip(start)
                                                                  .Take(count)
                                                                  .ToList()
                                                                  .ConvertAll(image =>
                                                                    {
                                                                        return new IOImageVariationsModel()
                                                                        {
                                                                            ID = image.ID,
                                                                            FileName = image.FileName,
                                                                            Width = image.Width,
                                                                            Height = image.Height,
                                                                            Scale = image.Scale
                                                                        };
                                                                    });

            return new Tuple<int, IList<IOImageVariationsModel>>(imageCount, paginatedImages);
        }

        public IList<IOImageVariationsModel> SaveImage(string fileData, string fileType, string contentType, string globalFileName, IList<IOImageVariationsModel> sizes)
        {
            byte[] imageData = Convert.FromBase64String(fileData);
            MemoryStream ms = new MemoryStream(imageData);
            Image rawImage = Image.FromStream(ms);
            Bitmap bitmapImage = new Bitmap(rawImage);
            List<IOImageVariationsModel> imagesList = new List<IOImageVariationsModel>();

            foreach (IOImageVariationsModel variation in sizes)
            {
                string variationFileName = globalFileName + "-" + variation.FileName + "." + fileType;
                byte[] scaledImage = ResizedImageFromRequest(bitmapImage, variation);
                Task<bool> uploadStatus = UploadToBlob(variationFileName, contentType, scaledImage);
                uploadStatus.Wait();

                if (uploadStatus.Result)
                {
                    IOImagesEntity imageEntity = new IOImagesEntity()
                    {
                        FileName = variationFileName,
                        FileType = contentType,
                        Width = variation.Width,
                        Height = variation.Height,
                        Scale = variation.Scale
                    };
                    DatabaseContext.Add(imageEntity);
                    DatabaseContext.SaveChanges();

                    IOImageVariationsModel responseVariation = variation;
                    responseVariation.FileName = variationFileName;
                    responseVariation.ID = imageEntity.ID;
                    imagesList.Add(responseVariation);
                }
            }

            return imagesList;
        }

        public async Task<bool> UploadToBlob(string filename, string contentType, byte[] imageBuffer)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = Configuration.GetConnectionString(IOConfigurationConstants.AzureStorageConnectionStringKey);
            string containerName = Configuration.GetValue<string>(IOConfigurationConstants.AzureStorageBlobNameKey);

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName.ToLower());
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                    cloudBlockBlob.Properties.ContentType = contentType;

                    // OPTION A: use imageBuffer (converted from memory stream)
                    await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);

                    return true;
                }
                catch (StorageException e)
                {
                    Logger.LogDebug("{0}", e.StackTrace);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Helper Methods

        private byte[] ResizedImageFromRequest(Bitmap fileData, IOImageVariationsModel imageData)
        {
            Image.GetThumbnailImageAbort thumbnailCallback = new Image.GetThumbnailImageAbort(() => false);
            int newWidth = imageData.Width ?? 0;
            int newHeight = imageData.Height ?? 0;
            bool keepRatio = imageData.KeepRatio;
            if (keepRatio)
            {
                float originalRatio = (float)fileData.Width / (float)fileData.Height;
                int ratioWidth = (int)(originalRatio * (float)newHeight);
                int ratioHeight = (int)(originalRatio / (float)newWidth);

                if (ratioHeight < newHeight)
                {
                    newWidth = ratioWidth;
                }
                else if (ratioWidth < newWidth)
                {
                    newHeight = ratioHeight;
                }
            }

            Image resizedImage = fileData.GetThumbnailImage(newWidth, newHeight, thumbnailCallback, IntPtr.Zero);

            var ms = new MemoryStream();
            resizedImage.Save(ms, ImageFormat.Png);
            ms.Flush();

            return ms.ToArray();
        }

        #endregion
    }
}
