using System;
using IOBootstrap.Net.Common.Messages.MW;
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

            return responseModel.Items;
        }

        public IList<IOMessageModel> GetAllMessages()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel();
            IOMWListResponseModel<IOMessageModel> responseModel = MWConnector.Get<IOMWListResponseModel<IOMessageModel>>(controller + "/" + "ListAllMessages", requestModel);

            return responseModel.Items;
        }

        public void AddMessage(IOMessageAddRequestModel request) 
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            MWConnector.Get<IOResponseModel>(controller + "/" + "AddMessagesItem", request);
        }

        public void DeleteMessage(int messageId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = messageId
            };
            MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteMessagesItem", requestModel);
        }

        public void UpdateMessage(IOMessageUpdateRequestModel request)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            MWConnector.Get<IOResponseModel>(controller + "/" + "UpdateMessagesItem", request);
        }
    }
}
