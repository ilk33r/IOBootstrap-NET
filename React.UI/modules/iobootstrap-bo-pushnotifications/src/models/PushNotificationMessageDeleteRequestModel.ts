import { BaseRequestModel } from "iobootstrap-ui-base";

class PushNotificationMessageDeleteRequestModel extends BaseRequestModel {

    id: number;

    constructor() {
        super();

        this.id = 0;
    }
}

export default PushNotificationMessageDeleteRequestModel;
