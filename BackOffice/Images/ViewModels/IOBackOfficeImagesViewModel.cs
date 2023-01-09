using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;

namespace IOBootstrap.NET.BackOffice.Images.ViewModels
{
    public class IOBackOfficeImagesViewModel<TDBContext> : IOBackOfficeViewModel<TDBContext>
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

        public IList<IOImageVariationsModel> SaveImages(string fileData, string fileType, string contentType, string globalFileName, IList<IOImageVariationsModel> sizes)
        {
            byte[] imageData = Convert.FromBase64String(fileData);
            MemoryStream ms = new MemoryStream(imageData);
            List<IOImageVariationsModel> imagesList = new List<IOImageVariationsModel>();

            foreach (IOImageVariationsModel variation in sizes)
            {
                string variationFileName = globalFileName + "-" + variation.FileName + ".png";
                Image rawImage = Image.Load(ms);
                byte[] scaledImage = ResizedImageFromRequest(rawImage, variation);
                Task<bool> uploadStatus = UploadToBlob(variationFileName, "image/png", scaledImage);
                uploadStatus.Wait();

                if (uploadStatus.Result)
                {
                    IOImageVariationsModel requestVariation = new IOImageVariationsModel()
                    {
                        FileName = variationFileName,
                        FileType = "image/png",
                        Width = variation.Width,
                        Height = variation.Height,
                        Scale = variation.Scale
                    };

                    imagesList.Add(requestVariation);
                } else {
                    throw new IOImageNotFoundException();
                }
            }

            List<IOImagesEntity> images = new List<IOImagesEntity>();

            foreach (IOImageVariationsModel variationsModel in imagesList)
            {
                IOImagesEntity imageEntity = new IOImagesEntity()
                {
                    FileName = variationsModel.FileName,
                    FileType = variationsModel.FileType,
                    Width = variationsModel.Width,
                    Height = variationsModel.Height,
                    Scale = variationsModel.Scale
                };

                DatabaseContext.Add(imageEntity);
                images.Add(imageEntity);
            }

            DatabaseContext.SaveChanges();

            return images.ConvertAll(i => new IOImageVariationsModel()
            {
                ID = i.ID,
                FileName = i.FileName,
                Width = i.Width,
                Height = i.Height,
                Scale = i.Scale
            });
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

        private byte[] ResizedImageFromRequest(Image image, IOImageVariationsModel imageData)
        {
            int newWidth = imageData.Width ?? 0;
            int newHeight = imageData.Height ?? 0;
            bool keepRatio = imageData.KeepRatio;
            if (keepRatio)
            {
                float originalRatio = (float)image.Width / (float)image.Height;
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

            image.Mutate(im => im.Resize(newWidth, newHeight));

            var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            ms.Flush();

            return ms.ToArray();
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
