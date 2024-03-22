using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Extensions;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Core.Interfaces;

namespace IOBootstrap.NET.BackOffice.Images.ViewModels
{
    public class IOBackOfficeImagesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOImageViewModel, IIOImageAssetViewModel
    where TDBContext : IODatabaseContext<TDBContext> 
    {
        #region Initialization Methods

        public IOBackOfficeImagesViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public IOGetImagesResponseModel GetImages(IOGetImagesRequestModel requestModel)
        {
            IQueryable<IOImagesEntity> images = DatabaseContext.Images;
            int imageCount = images.Count();
            IList<IOImageVariationsModel> paginatedImages = images
                                                                .Select(i => new IOImageVariationsModel()
                                                                {
                                                                    ID = i.ID,
                                                                    FileName = i.FileName,
                                                                    Width = i.Width,
                                                                    Height = i.Height,
                                                                    Scale = i.Scale
                                                                })
                                                                .OrderBy(i => i.ID)
                                                                .Skip(requestModel.Start)
                                                                .Take(requestModel.Count)
                                                                .ToList();

            foreach (IOImageVariationsModel image in paginatedImages)
            {
                image.FileName = this.CreateImagePublicId(image.FileName);
            }

            return new IOGetImagesResponseModel(imageCount, paginatedImages);
        }

        public IOImageVariationsModel SaveImagesMetaData(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            if (!File.Exists(filePath))
            {
                throw new IOImageNotFoundException();
            }

            FileStream fs = File.OpenRead(filePath);
            Image rawImage = Image.Load(fs);

            IOImagesEntity imageEntity = new IOImagesEntity()
            {
                FileName = fileName,
                FileType = "image/jpeg",
                Width = rawImage.Width,
                Height = rawImage.Height,
                Scale = null
            };

            DatabaseContext.Add(imageEntity);
            DatabaseContext.SaveChanges();

            return new IOImageVariationsModel()
            {
                ID = imageEntity.ID,
                FileName = imageEntity.FileName,
                Width = imageEntity.Width,
                Height = imageEntity.Height,
                Scale = imageEntity.Scale
            };
        }

        public void DeleteImage(IODeleteImagesRequestModel requestModel)
        {
            IOImagesEntity imagesEntity = DatabaseContext.Images.Find(requestModel.ImageId);
            if (imagesEntity == null)
            {
                throw new IOImageNotFoundException();
            }

            this.RemoveFile(imagesEntity.FileName);
            DatabaseContext.Remove(imagesEntity);
            DatabaseContext.SaveChanges();
        }

        #endregion

        #region Azure Storage

        public async Task<bool> DeleteFromBlob(string filename)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            string containerName = Configuration.GetValue<string>(IOConfigurationConstants.AzureStorageBlobNameKey);
            try
            {
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                if (containerClient == null) {
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                }
                
                // Get a reference to a blob
                await containerClient.DeleteBlobIfExistsAsync(filename);
                return true;
            }
            catch (Exception e)
            {
                Logger.LogDebug("{0}", e.StackTrace);
                return false;
            }
        }

        #endregion

        #region Helper Methods

        private BlobServiceClient GetBlobServiceClient() 
        {
            string storageConnectionString = Configuration.GetConnectionString(IOConfigurationConstants.AzureStorageConnectionStringKey);
            return new BlobServiceClient(storageConnectionString);
        }

        private async Task<bool> UploadToBlob(string filename, string contentType, byte[] imageBuffer)
        {
            BlobServiceClient blobServiceClient = GetBlobServiceClient();
            string containerName = Configuration.GetValue<string>(IOConfigurationConstants.AzureStorageBlobNameKey);
            try
            {
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                if (containerClient == null) {
                    containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
                }
                
                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(filename);
                using(var ms = new MemoryStream(imageBuffer, false))
                {
                    await blobClient.UploadAsync(ms, new BlobHttpHeaders{ ContentType = contentType});
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.LogDebug("{0}", e.StackTrace);
                return false;
            }
        }

        #endregion
    }
}
