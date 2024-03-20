import { BaseRequestModel } from "iobootstrap-ui-base";

class MessageDeleteRequestModel extends BaseRequestModel {

    messageId: number;

    constructor() {
        super();

        this.messageId = 0;
    }
}

export default MessageDeleteRequestModel;
