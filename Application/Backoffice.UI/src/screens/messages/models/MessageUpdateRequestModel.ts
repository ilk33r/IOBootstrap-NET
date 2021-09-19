import MessageAddRequestModel from "./MessageAddRequestModel";

class MessageUpdateRequestModel extends MessageAddRequestModel {

    messageId: number;

    constructor() {
        super();

        this.messageId = 0;
    }
}

export default MessageUpdateRequestModel;
