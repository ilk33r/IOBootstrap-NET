import { BaseRequestModel, DeviceTypes } from "iobootstrap-ui-base";

class SendPushNotificationRequestModel extends BaseRequestModel {

	clientId: number | null;
	deviceType: DeviceTypes;
	notificationCategory: string | null;
	notificationData: string | null;
	notificationMessage: string;
	notificationTitle: string;

	constructor() {
		super();

		this.clientId = null;
		this.deviceType = DeviceTypes.Unkown;
		this.notificationCategory = null;
		this.notificationData = null;
		this.notificationMessage = "";
		this.notificationTitle = "";
	}
}

export default SendPushNotificationRequestModel;
