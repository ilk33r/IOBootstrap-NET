import { BaseResponseModel } from "iobootstrap-ui-base";
import PushNotificationMessageModel from "./PushNotificationMessageModel";

class ListPushNotificationMessageResponseModel extends BaseResponseModel {

    count: number;
    messages: PushNotificationMessageModel[];

    constructor() {
        super();

        this.count = 0;
        this.messages = [];
    }
}

export default ListPushNotificationMessageResponseModel;
