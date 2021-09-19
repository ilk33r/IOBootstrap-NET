class MessageModel {

    id: number;
    message: string;
    messageCreateDate: string | null;
    messageStartDate: string | null;
    messageEndDate: string | null;

    constructor() {
        this.id = 0;
        this.message = "";
        this.messageCreateDate = null;
        this.messageStartDate = null;
        this.messageEndDate = null;
    }
}

export default MessageModel;
