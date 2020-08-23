using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Enumerations;
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

        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOGetImagesResponseModel GetImages([FromBody] IOGetImagesRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                // Then return validation error
                return new IOGetImagesResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            Tuple<int, IList<IOImageVariationsModel>> images = ViewModel.GetImages(requestModel.Start, requestModel.Count);
            IOGetImagesResponseModel responseModel = new IOGetImagesResponseModel(IOResponseStatusMessages.OK);
            responseModel.Count = images.Item1;
            responseModel.Images = images.Item2;
            return responseModel;
        }

        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOResponseModel DeleteImages([FromBody] IODeleteImagesRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                // Then return validation error
                return new IOResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            if (ViewModel.DeleteImages(requestModel.ImagesIdList))
            {
                return new IOResponseModel(IOResponseStatusMessages.OK);
            }

            return new IOResponseModel(IOResponseStatusMessages.BAD_REQUEST);
        }

        [IOUserRole(UserRoles.CustomUser)]
        [HttpPost]
        public IOSaveImageResponseModel SaveImages([FromBody] IOSaveImageRequestModel requestModel)
        {
            // Validate request
            if (!ModelState.IsValid)
            {
                // Then return validation error
                return new IOSaveImageResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Add File
            Regex rx = new Regex(@"data:image\/([a-zA-Z]+);base64,(.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(requestModel.FileData);
            if (matches.Count != 1)
            {
                // Then return validation error
                return new IOSaveImageResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            GroupCollection groups = matches[0].Groups;
            if (groups.Count != 3)
            {
                // Then return validation error
                return new IOSaveImageResponseModel(IOResponseStatusMessages.BAD_REQUEST);
            }

            // Obtain values
            string fileType = groups[1].Value;
            string fileData = groups[2].Value;
            string contentType = "image/" + fileType;
            string globalFileName = IORandomUtilities.GenerateGUIDString();

            IList<IOImageVariationsModel> imageList = ViewModel.SaveImage(fileData, fileType, contentType, globalFileName, requestModel.Sizes);

            // Create and return response
            return new IOSaveImageResponseModel(IOResponseStatusMessages.OK, imageList);
        }

        #endregion
    }
}
