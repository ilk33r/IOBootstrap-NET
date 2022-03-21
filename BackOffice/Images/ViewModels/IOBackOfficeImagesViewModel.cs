using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Images;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace IOBootstrap.NET.BackOffice.Images.ViewModels
{
    public class IOBackOfficeImagesViewModel : IOBackOfficeViewModel
    {
        #region Initialization Methods

        public IOBackOfficeImagesViewModel() : base()
        {
        }

        #endregion

        #region View Model Methods

        public IOGetImagesResponseModel GetImages(IOGetImagesRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            IOMWListResponseModel<IOImageVariationsModel> response = MWConnector.Get<IOMWListResponseModel<IOImageVariationsModel>>(controller + "/" + "GetImages", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });

            return new IOGetImagesResponseModel(response.Count ?? 0, response.Items);
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

            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            IOMWSaveImageRequestModel requestModel = new IOMWSaveImageRequestModel()
            {
                Variations = imagesList
            };
            IOMWListResponseModel<IOImageVariationsModel> response = MWConnector.Get<IOMWListResponseModel<IOImageVariationsModel>>(controller + "/" + "SaveImages", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });

            return response.Items;
        }

        public void DeleteImages(IODeleteImagesRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            IOMWListResponseModel<IOImageVariationsModel> response = MWConnector.Get<IOMWListResponseModel<IOImageVariationsModel>>(controller + "/" + "DeleteImages", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });

            if (response.Items == null)
            {
                throw new IOImageNotFoundException();
            }

            foreach (IOImageVariationsModel variation in response.Items)
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
