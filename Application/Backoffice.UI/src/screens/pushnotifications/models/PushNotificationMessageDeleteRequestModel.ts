import BaseRequestModel from "../../../common/models/BaseRequestModel";

class PushNotificationMessageDeleteRequestModel extends BaseRequestModel {

    id: number;

    constructor() {
        super();

        this.id = 0;
    }
}

export default PushNotificationMessageDeleteRequestModel;
