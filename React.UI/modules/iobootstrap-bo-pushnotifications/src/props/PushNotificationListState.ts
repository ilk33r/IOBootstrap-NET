import PushNotificationMessageModel from "../models/PushNotificationMessageModel";

class PushNotificationListState {

    count: number;
    messages: PushNotificationMessageModel[];

    constructor() {
        this.count = 0;
        this.messages = [];
    }
}

export default PushNotificationListState;
