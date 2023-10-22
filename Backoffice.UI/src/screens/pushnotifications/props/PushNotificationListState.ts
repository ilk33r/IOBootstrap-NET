import PushNotificationMessageModel from "../models/PushNotificationMessageModel";

class PushNotificationListState {

    messages: PushNotificationMessageModel[];

    constructor() {
        this.messages = [];
    }
}

export default PushNotificationListState;
