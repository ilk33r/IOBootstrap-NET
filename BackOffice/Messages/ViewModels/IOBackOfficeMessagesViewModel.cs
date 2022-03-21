using System;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.Messages;
using IOBootstrap.NET.Common.Models.Messages;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.Messages.ViewModels
{
    public class IOBackOfficeMessagesViewModel : IOBackOfficeViewModel
    {

        #region Initialization Methods

        public IOBackOfficeMessagesViewModel() : base()
        {
        }

        #endregion

        public IList<IOMessageModel> GetMessages()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel();
            IOMWListResponseModel<IOMessageModel> responseModel = MWConnector.Get<IOMWListResponseModel<IOMessageModel>>(controller + "/" + "ListMessages", requestModel);
            if (MWConnector.HandleResponse(responseModel, code => {}))
            {
                // Return response
                return responseModel.Items;
            }

            return new List<IOMessageModel>();
        }

        public IList<IOMessageModel> GetAllMessages()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel();
            IOMWListResponseModel<IOMessageModel> responseModel = MWConnector.Get<IOMWListResponseModel<IOMessageModel>>(controller + "/" + "ListAllMessages", requestModel);
            if (MWConnector.HandleResponse(responseModel, code => {}))
            {
                return responseModel.Items;
            }

            return new List<IOMessageModel>();
        }

        public void AddMessage(IOMessageAddRequestModel request) 
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "AddMessagesItem", request);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }

        public void DeleteMessage(int messageId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = messageId
            };
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteMessagesItem", requestModel);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }

        public void UpdateMessage(IOMessageUpdateRequestModel request)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateMessagesItem", request);
            MWConnector.HandleResponse(response, code => {
                // Return response
                throw new IOMWConnectionException();
            });
        }
    }
}
