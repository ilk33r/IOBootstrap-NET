using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Extensions;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.BackOffice.Images.ViewModels
{
    public class IOBackOfficeImagesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>, IIOImageViewModel
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

            return new IOGetImagesResponseModel(imageCount, paginatedImages);
        }

        public IOImageVariationsModel SaveImagesMetaData(string filePath, string fileName)
        {
            string globalFileName = IORandomUtilities.GenerateGUIDString();
            string variationFileName = globalFileName + "-" + fileName.RemoveNonASCII() + ".jpg";

            if (!File.Exists(filePath))
            {
                throw new IOImageNotFoundException();
            }

            FileStream fs = File.OpenRead(filePath);
            Image rawImage = Image.Load(fs);

            IOImagesEntity imageEntity = new IOImagesEntity()
            {
                FileName = variationFileName,
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

        public void DeleteImages(IODeleteImagesRequestModel requestModel)
        {
            List<IOImageVariationsModel> imageVariations = new List<IOImageVariationsModel>();

            foreach (int imageId in requestModel.ImagesIdList)
            {
                IOImagesEntity imagesEntity = DatabaseContext.Images.Find(imageId);
                if (imagesEntity == null)
                {
                    continue;
                }

                imageVariations.Add(new IOImageVariationsModel()
                {
                    ID = imagesEntity.ID,
                    FileName = new string(imagesEntity.FileName),
                    FileType = imagesEntity.FileType,
                    Width = imagesEntity.Width,
                    Height = imagesEntity.Height,
                    Scale = imagesEntity.Scale
                });

                DatabaseContext.Remove(imagesEntity);
            }

            if (imageVariations.Count() > 0)
            {
                DatabaseContext.SaveChanges();
            }
            else
            {
                throw new IOImageNotFoundException();
            }

            foreach (IOImageVariationsModel variation in imageVariations)
            {
                try {
                    Task<bool> deleteStatus = DeleteFromBlob(variation.FileName);
                    deleteStatus.Wait();

                    if (!deleteStatus.Result)
                    {
                        throw new IOImageDeleteException();
                    }
                }
                catch (Exception ex)
                {
                    throw new IOImageDeleteException(ex.Message);
                }
            }
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
