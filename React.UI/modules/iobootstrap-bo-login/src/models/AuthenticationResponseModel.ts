import { BaseResponseModel } from "iobootstrap-ui-base";

class AuthenticationResponseModel extends BaseResponseModel {

    token: string | null;
    extras: string | null;
    userRole: number | null;

    constructor() {
        super();

        this.token = null;
        this.extras = null;
        this.userRole = null;
    }
}

export default AuthenticationResponseModel;
