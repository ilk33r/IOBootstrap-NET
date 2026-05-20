class PushNotificationMessageModel {

    id: number;
    deviceType: number | null;
    notificationCategory: string | null;
    notificationData: string | null;
    notificationMessage: string | null;
    notificationTitle: string;
    isCompleted: boolean;
    createdBy: string | null;
    createdDate: string | null;
    updateDate: string | null;
    deliveredDevicesCount: number | null;
    
    constructor() {
        this.id = 0;
        this.deviceType = null;
        this.notificationCategory = null;
        this.notificationData = null;
        this.notificationMessage = "";
        this.notificationTitle = "";
        this.isCompleted = false;
        this.createdBy = null;
        this.createdDate = null;
        this.updateDate = null;
        this.deliveredDevicesCount = null;
    }
}

export default PushNotificationMessageModel;
