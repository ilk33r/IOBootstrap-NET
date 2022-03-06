using System;
using IOBootstrap.Net.Common.Exceptions.Common;
using IOBootstrap.Net.Common.Messages.MW;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Models.PushNotification;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.BackOffice.PushNotification.ViewModels
{
    public class IOPushNotificationBackOfficeViewModel : IOBackOfficeViewModel
    {
        
        #region Initialization Methods

        public IOPushNotificationBackOfficeViewModel()
        {
        }

        #endregion

        #region Back Office Methods

        public virtual IList<PushNotificationMessageModel> ListMessages()
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePushNotificationControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel();
            IOMWListResponseModel<PushNotificationMessageModel> response = MWConnector.Get<IOMWListResponseModel<PushNotificationMessageModel>>(controller + "/" + "ListMessages", requestModel);

            if (MWConnector.HandleResponse(response, code => {}))
            {
                return response.Items;
            }

            return new List<PushNotificationMessageModel>();
        }

        public void SendNotifications(SendPushNotificationRequestModel requestModel)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePushNotificationControllerNameKey);
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "SendNotification", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });
        }

        public void DeleteMessage(int messageId)
        {
            string controller = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePushNotificationControllerNameKey);
            IOMWFindRequestModel requestModel = new IOMWFindRequestModel()
            {
                ID = messageId
            };
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "DeleteMessage", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });
        }

        #endregion
    }
}
