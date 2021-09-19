import BaseRequestModel from "../../../common/models/BaseRequestModel";

class ClientDeleteRequestModel extends BaseRequestModel {

    ClientId: number;

    constructor() {
        super();

        this.ClientId = 0;
    }
}

export default ClientDeleteRequestModel;
