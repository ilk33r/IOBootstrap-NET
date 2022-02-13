using System;
using IOBootstrap.NET.Core.ViewModels;

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

        //TODO: Migrate with MW.
        /*
        public Tuple<int, IList<IOImageVariationsModel>> GetImages(int start, int count)
        {
            IQueryable<IOImagesEntity> images = DatabaseContext.Images;
            int imageCount = images.Count();
            IList<IOImageVariationsModel> paginatedImages = images.OrderBy(i => i.ID)
                                                                  .Skip(start)
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

        public void DeleteImages(IList<int> imageIdList)
        {
            foreach (int imageId in imageIdList)
            {
                IOImagesEntity imagesEntity = DatabaseContext.Images.Find(imageId);

                if (imagesEntity == null)
                {
                    throw new IOImageNotFoundException();
                }

                string imageName = new string(imagesEntity.FileName);
                try {
                    DatabaseContext.Remove(imagesEntity);
                    DatabaseContext.SaveChanges();
                    Task<bool> deleteStatus = DeleteFromBlob(imageName);
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
        */
        #endregion
    }
}
