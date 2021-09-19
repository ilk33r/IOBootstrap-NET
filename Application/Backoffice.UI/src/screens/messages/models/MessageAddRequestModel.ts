import BaseRequestModel from "../../../common/models/BaseRequestModel";

class MessageAddRequestModel extends BaseRequestModel {

    message: string;
    messageStartDate: string | null;
    messageEndDate: string | null;

    constructor() {
        super();
        this.message = "";
        this.messageStartDate = null;
        this.messageEndDate = null;
    }
}

export default MessageAddRequestModel;
