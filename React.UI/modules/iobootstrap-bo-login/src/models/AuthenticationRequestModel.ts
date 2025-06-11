import { BaseRequestModel } from "iobootstrap-ui-base";

class AuthenticationRequestModel extends BaseRequestModel {

    UserName: string | undefined;
    Password: string | undefined;
    CaptchaID: string | undefined | null;
    EncryptedCaptcha: string | undefined | null;
}

export default AuthenticationRequestModel;
