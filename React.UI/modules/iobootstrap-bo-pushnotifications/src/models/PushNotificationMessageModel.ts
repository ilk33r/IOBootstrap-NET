class PushNotificationMessageModel {

    id: number;
    deviceType: number | null;
    notificationCategory: string | null;
    notificationData: string | null;
    notificationDate: string;
    notificationMessage: string;
    notificationTitle: string;
    isCompleted: boolean;
    
    constructor() {
        this.id = 0;
        this.deviceType = null;
        this.notificationCategory = null;
        this.notificationData = null;
        this.notificationDate = "";
        this.notificationMessage = "";
        this.notificationTitle = "";
        this.isCompleted = false;
    }
}

export default PushNotificationMessageModel;
