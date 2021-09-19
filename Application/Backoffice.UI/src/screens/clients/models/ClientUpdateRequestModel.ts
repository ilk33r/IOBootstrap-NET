import BaseRequestModel from "../../../common/models/BaseRequestModel";

class ClientUpdateRequestModel extends BaseRequestModel {

    ClientId: number;
    ClientDescription: string;
    IsEnabled: number;
    RequestCount: number;
    MaxRequestCount: number;

    constructor() {
        super();

        this.ClientId = 0;
        this.ClientDescription = "";
        this.IsEnabled = 0;
        this.RequestCount = 0;
        this.MaxRequestCount = 0;
    }
}

export default ClientUpdateRequestModel;
