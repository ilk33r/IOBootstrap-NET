import { BaseRequestModel } from "iobootstrap-ui-base";

class CheckTokenRequestModel extends BaseRequestModel {

    Token: string | null | undefined;
    Extras: string | null | undefined;
}

export default CheckTokenRequestModel;
