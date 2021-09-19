import BaseRequestModel from "../../../common/models/BaseRequestModel";

class MessageDeleteRequestModel extends BaseRequestModel {

    messageId: number;

    constructor() {
        super();

        this.messageId = 0;
    }
}

export default MessageDeleteRequestModel;
