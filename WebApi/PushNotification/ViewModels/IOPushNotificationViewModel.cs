using System;
using IOBootstrap.Net.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Messages.PushNotification;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;

namespace IOBootstrap.NET.WebApi.PushNotification.ViewModels
{
    public class IOPushNotificationViewModel : IOViewModel
    {
		public void AddTokenV2(AddPushNotificationRequestModel requestModel) 
        {
			IOAESUtilities aesUtility = GetAesUtility();
			AddPushNotificationRequestModel request = requestModel;
			request.DeviceId = aesUtility.Decrypt(requestModel.DeviceId);
			request.DeviceToken = aesUtility.Decrypt(requestModel.DeviceToken);
			request.ClientId = ClientId;

            string controller = Configuration.GetValue<string>(IOConfigurationConstants.PushNotificationControllerNameKey);
            IOResponseModel response = MWConnector.Get<IOResponseModel>(controller + "/" + "AddPushNotificationTokenV2", requestModel);
            MWConnector.HandleResponse(response, code => {
                throw new IOMWConnectionException();
            });
		}
    }
}
