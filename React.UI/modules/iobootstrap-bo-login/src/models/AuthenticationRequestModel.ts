import { BaseRequestModel } from "iobootstrap-ui-base";

class AuthenticationRequestModel extends BaseRequestModel {

    UserName: string | undefined;
    Password: string | undefined;
}

export default AuthenticationRequestModel;
