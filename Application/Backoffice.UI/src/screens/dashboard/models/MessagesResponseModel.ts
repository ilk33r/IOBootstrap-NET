import BaseResponseModel from "../../../common/models/BaseResponseModel";
import MessageModel from "./MessageModel";

class MessagesResponseModel extends BaseResponseModel {

    messages: MessageModel[] | undefined;
}

export default MessagesResponseModel;
