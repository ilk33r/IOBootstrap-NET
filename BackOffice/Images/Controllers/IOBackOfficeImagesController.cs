using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Enumerations;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Images;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Images.Controllers
{
    public class IOBackOfficeImagesController<TViewModel, TDBContext> : IOBackOfficeController<TViewModel, TDBContext> where TViewModel : IOBackOfficeImagesViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Controller Lifecycle

        public IOBackOfficeImagesController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region API Methods

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOGetImagesResponseModel GetImages([FromBody] IOGetImagesRequestModel requestModel)
        {
            Tuple<int, IList<IOImageVariationsModel>> images = ViewModel.GetImages(requestModel.Start, requestModel.Count);
            IOGetImagesResponseModel responseModel = new IOGetImagesResponseModel(images.Item1, images.Item2);
            return responseModel;
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOResponseModel DeleteImages([FromBody] IODeleteImagesRequestModel requestModel)
        {
            ViewModel.DeleteImages(requestModel.ImagesIdList);
            return new IOResponseModel();
        }

        [IOValidateRequestModel]
        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOSaveImageResponseModel SaveImages([FromBody] IOSaveImageRequestModel requestModel)
        {
            // Add File
            Regex rx = new Regex(@"data:image\/([a-zA-Z]+);base64,(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(requestModel.FileData);
            if (matches.Count != 1)
            {
                // Then return validation error
                throw new IOInvalidRequestException();
            }

            GroupCollection groups = matches[0].Groups;
            if (groups.Count != 3)
            {
                // Then return validation error
                throw new IOInvalidRequestException();
            }

            // Obtain values
            string fileType = groups[1].Value;
            string fileData = groups[2].Value;
            string contentType = "image/" + fileType;
            string globalFileName = IORandomUtilities.GenerateGUIDString();

            IList<IOImageVariationsModel> imageList = ViewModel.SaveImage(fileData, fileType, contentType, globalFileName, requestModel.Sizes);

            // Create and return response
            return new IOSaveImageResponseModel(imageList);
        }

        #endregion
    }
}
