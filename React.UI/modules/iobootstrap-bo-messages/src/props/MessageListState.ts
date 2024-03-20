import MessageModel from "../models/MessageModel";

class MessageListState {

    messages: MessageModel[];

    constructor() {
        this.messages = [];
    }
}

export default MessageListState;
