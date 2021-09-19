import BaseResponseModel from "../../../common/models/BaseResponseModel";
import MessageModel from "./MessageModel";

class MessageListResponseModel extends BaseResponseModel {

    messages: MessageModel[];

    constructor() {
        super();
        this.messages = [];
    }
}

export default MessageListResponseModel;
