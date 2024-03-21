import { BaseResponseModel } from "iobootstrap-ui-base";
import MessageModel from "./MessageModel";

class MessagesResponseModel extends BaseResponseModel {

    messages: MessageModel[] | undefined;
}

export default MessagesResponseModel;
