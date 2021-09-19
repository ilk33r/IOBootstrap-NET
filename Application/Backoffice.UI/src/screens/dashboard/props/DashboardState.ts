import MessageModel from "../models/MessageModel";

class DashboardState {

    messages: MessageModel[];

    constructor() {
        this.messages = [];
    }
}

export default DashboardState;
