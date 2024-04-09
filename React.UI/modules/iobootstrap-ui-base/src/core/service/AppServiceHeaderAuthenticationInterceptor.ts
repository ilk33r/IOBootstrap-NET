import UICommonConstants from "../../common/constants/UICommonConstants";
import AppCryptography from "../cryptography/AppCryptography";
import AppStorage from "../storage/AppStorage";
import { IAppServiceHeaderInterceptor } from "./IAppServiceHeaderInterceptor";

class AppServiceHeaderAuthenticationInterceptor implements IAppServiceHeaderInterceptor {

    private authorization: string;
    private clientID: string;
    private clientSecret: string;
    private keyID: string | null;
    private symmetricKey: string | null;
    private symmetricIV: string | null;

    constructor() {
        this.authorization = "";
        this.clientID = "";
        this.clientSecret = "";
    }

    public initialize(authorization: string, clientID: string, clientSecret: string) {
        this.authorization = authorization;
        this.clientID = clientID;
        this.clientSecret = clientSecret;
    }

    public setKeyID(keyID: string | null) {
        this.keyID = keyID;
    }

    public setSymmetricKeys(symmetricKey: string | null, symmetricIV: string | null) {
        this.symmetricKey = symmetricKey;
        this.symmetricIV = symmetricIV;
    }

    public interceptHeaders(): Record<string, string> {
        let headers: Record<string, string> = {
            'X-IO-AUTHORIZATION': this.authorization,
            'X-IO-CLIENT-ID': this.clientID,
            'X-IO-CLIENT-SECRET': this.clientSecret
        };

        if (this.keyID != null) {
            headers['X-KEY-ID'] = this.keyID;
        }

        if (this.symmetricKey != null) {
            headers['X-SYMMETRIC-KEY'] = this.symmetricKey;
        }

        if (this.symmetricIV != null) {
            headers['X-SYMMETRIC-IV'] = this.symmetricIV;
        }

        if (!AppCryptography.Instance.initilized) {
            return headers;
        }

        const userToken = AppStorage.Instance.stringForKey(UICommonConstants.userTokenStorageKey);
        if (userToken == null) {
            return headers;
        }

        headers['X-IO-AUTHORIZATION-TOKEN'] = userToken;
        return headers;
    }
}

export default AppServiceHeaderAuthenticationInterceptor;
