import { BaseResponseModel } from "iobootstrap-ui-base";

class CheckTokenResponseModel extends BaseResponseModel {

    userName: string | null;
    userRole: number | null;

    constructor() {
        super();

        this.userName = null;
        this.userRole = null;
    }
}

export default CheckTokenResponseModel;
