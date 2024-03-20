import { BaseResponseModel } from "iobootstrap-ui-base";
import MessageModel from "./MessageModel";

class MessageListResponseModel extends BaseResponseModel {

    messages: MessageModel[];

    constructor() {
        super();
        this.messages = [];
    }
}

export default MessageListResponseModel;
