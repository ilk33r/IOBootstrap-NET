import { BaseRequestModel } from "iobootstrap-ui-base";

class IOLogoutRequestModel extends BaseRequestModel {

    userName: string | null;

    constructor() {
        super();

        this.userName = null;
    }
}

export default IOLogoutRequestModel;
