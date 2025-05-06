import UICommonConstants from "../../common/constants/UICommonConstants";
import AppCryptography from "../cryptography/AppCryptography";
import AppStorage from "../storage/AppStorage";
import { IAppServiceHeaderInterceptor } from "./IAppServiceHeaderInterceptor";

class AppServiceHeaderAuthenticationInterceptor implements IAppServiceHeaderInterceptor {

    private authorization: string;
    private keyID: string | null;
    private sessionID: string | null;
    private symmetricKey: string | null;
    private symmetricIV: string | null;
    private nonce: string | null;

    constructor() {
        this.authorization = "";
        this.keyID = null;
        this.sessionID = null;
        this.symmetricKey = null;
        this.symmetricIV = null;
        this.nonce = null;
    }

    public initialize(authorization: string) {
        this.authorization = authorization;
    }

    public setKeyID(keyID: string | null) {
        this.keyID = keyID;
    }

    public setSymmetricKeys(symmetricKey: string | null, symmetricIV: string | null) {
        this.symmetricKey = symmetricKey;
        this.symmetricIV = symmetricIV;
    }

    public interceptRequestHeaders(): Promise<Record<string, string>> {
        return this.getRequestHeaders();
    }

    public interceptResponseHeaders(headers: Headers): void {
        headers.forEach((value: string, key: string) => {
            if (key.toLowerCase() === "x-nonce") {
                this.nonce = value;
            }

            if (key.toLowerCase() === "x-session-id") {
                this.sessionID = value;
            }
        });
    }

    private async getRequestHeaders(): Promise<Record<string, string>> {
        let headers: Record<string, string> = {
            'X-IO-AUTHORIZATION': this.authorization
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

        if (this.sessionID != null) {
            headers['X-SESSION-ID'] = this.sessionID;
        }

        if (!AppCryptography.Instance.initilized) {
            return headers;
        }

        if (this.nonce != null) {
            headers["X-NONCE"] = await AppCryptography.Instance.encrypt(this.nonce)
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
