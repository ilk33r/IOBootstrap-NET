using System;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.MW.Core.ViewModels;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.DataAccess.Entities;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels
{
    public class IOBackOfficeImagesViewModel<TDBContext> : IOMWViewModel<TDBContext> where TDBContext : IODatabaseContext<TDBContext>
    {

        public IOMWListResponseModel<IOImageVariationsModel> GetImages(int start, int count)
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
                                                                .Skip(start)
                                                                .Take(count)
                                                                .ToList();

            IOMWListResponseModel<IOImageVariationsModel> responseModel = new IOMWListResponseModel<IOImageVariationsModel>(paginatedImages);
            responseModel.Count = imageCount;

            return responseModel;
        }

        public IList<IOImageVariationsModel> SaveImages(IOMWSaveImageRequestModel requestModel)
        {
            List<IOImagesEntity> images = new List<IOImagesEntity>();

            foreach (IOImageVariationsModel variationsModel in requestModel.Variations)
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

        public IList<IOImageVariationsModel> DeleteImages(IODeleteImagesRequestModel requestModel)
        {
            List<IOImageVariationsModel> imageVariations = new List<IOImageVariationsModel>();

            foreach (int imageId in requestModel.ImagesIdList)
            {
                IOImagesEntity imagesEntity = DatabaseContext.Images.Find(imageId);
                if (imagesEntity == null)
                {
                    return null;
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

            return imageVariations;
        }
    }
}
