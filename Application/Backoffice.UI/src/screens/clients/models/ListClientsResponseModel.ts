import BaseResponseModel from "../../../common/models/BaseResponseModel";
import ClientModel from "./ClientModel";

class ListClientsResponseModel extends BaseResponseModel {

    clientList: ClientModel[];

    constructor() {
        super();
        this.clientList = [];
    }
}

export default ListClientsResponseModel;
