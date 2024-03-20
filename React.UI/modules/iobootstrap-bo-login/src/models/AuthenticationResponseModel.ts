import { BaseResponseModel } from "iobootstrap-ui-base";

class AuthenticationResponseModel extends BaseResponseModel {

    token: string | null;
    userName: string | null;
    userRole: number | null;

    constructor() {
        super();

        this.token = null;
        this.userName = null;
        this.userRole = null;
    }
}

export default AuthenticationResponseModel;
