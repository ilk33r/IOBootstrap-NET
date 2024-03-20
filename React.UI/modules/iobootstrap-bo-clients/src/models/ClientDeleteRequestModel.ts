import { BaseRequestModel } from "iobootstrap-ui-base";

class ClientDeleteRequestModel extends BaseRequestModel {

    ClientId: number;

    constructor() {
        super();

        this.ClientId = 0;
    }
}

export default ClientDeleteRequestModel;
