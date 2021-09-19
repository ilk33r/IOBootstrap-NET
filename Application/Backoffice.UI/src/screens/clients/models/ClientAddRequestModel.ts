import BaseRequestModel from "../../../common/models/BaseRequestModel";

class ClientAddRequestModel extends BaseRequestModel {

    ClientDescription: string;
    RequestCount: number;

    constructor() {
        super();
        this.ClientDescription = "";
        this.RequestCount = 0;
    }
}

export default ClientAddRequestModel;
