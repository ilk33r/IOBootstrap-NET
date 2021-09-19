import ClientModel from "../models/ClientModel";

class ClientListState {

    clientList: ClientModel[];

    constructor() {
        this.clientList = [];
    }
}

export default ClientListState;
