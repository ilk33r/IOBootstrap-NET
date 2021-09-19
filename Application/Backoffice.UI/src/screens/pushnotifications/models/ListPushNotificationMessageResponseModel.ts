import BaseResponseModel from "../../../common/models/BaseResponseModel";
import PushNotificationMessageModel from "./PushNotificationMessageModel";

class ListPushNotificationMessageResponseModel extends BaseResponseModel {

    messages: PushNotificationMessageModel[];

    constructor() {
        super();

        this.messages = [];
    }
}

export default ListPushNotificationMessageResponseModel;
