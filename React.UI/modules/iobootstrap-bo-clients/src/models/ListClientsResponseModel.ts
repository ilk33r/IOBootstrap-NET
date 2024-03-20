import { BaseResponseModel } from "iobootstrap-ui-base";
import ClientModel from "./ClientModel";

class ListClientsResponseModel extends BaseResponseModel {

    clientList: ClientModel[];

    constructor() {
        super();
        this.clientList = [];
    }
}

export default ListClientsResponseModel;
