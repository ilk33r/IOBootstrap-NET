import { BaseResponseModel } from "iobootstrap-ui-base";

class HandshakeResponseModel extends BaseResponseModel {

    keyID: string | null;
    publicKeyExponent: string | null;
    publicKeyModulus: string | null;

    constructor() {
        super();

        this.keyID = null;
        this.publicKeyExponent = null;
        this.publicKeyModulus = null;
    }
}

export default HandshakeResponseModel;
