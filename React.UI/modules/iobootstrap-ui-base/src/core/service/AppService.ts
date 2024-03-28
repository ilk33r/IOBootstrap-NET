import AppStorage from "../storage/AppStorage";
import BaseRequestModel from "../../common/models/BaseRequestModel";
import BaseResponseModel from "../../common/models/BaseResponseModel";
import UICommonConstants from "../../common/constants/UICommonConstants";

type AppServiceBlobHandler = (blob: Blob) => void;
type AppServiceSuccessHandler<T extends BaseResponseModel> = (response: T) => void;
type AppServiceErrorHandler = (error: string) => void;

class AppService {

    public authorization: string;
    public baseUrl: string;
    public clientID: string;
    public clientSecret: string;

    private static _instance: AppService;

    private constructor() {
        this.authorization = "";
        this.baseUrl = "";
        this.clientID = "";
        this.clientSecret = "";
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public get<TResponse extends BaseResponseModel>(path: string, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const userToken = AppStorage.Instance.stringForKey(UICommonConstants.userTokenStorageKey);

        fetch(requestUrl, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'X-IO-AUTHORIZATION': this.authorization,
                'X-IO-AUTHORIZATION-TOKEN': (userToken == null) ? '' : userToken,
                'X-IO-CLIENT-ID': this.clientID,
                'X-IO-CLIENT-SECRET': this.clientSecret
            }
        })
        .then(response => response.json())
        .then(data => {
            const response = data as TResponse;
            successHandler(response);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public post<TResponse extends BaseResponseModel>(path: string, request: BaseRequestModel, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const userToken = AppStorage.Instance.stringForKey(UICommonConstants.userTokenStorageKey);

        fetch(requestUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-IO-AUTHORIZATION': this.authorization,
                'X-IO-AUTHORIZATION-TOKEN': (userToken == null) ? '' : userToken,
                'X-IO-CLIENT-ID': this.clientID,
                'X-IO-CLIENT-SECRET': this.clientSecret
            },
            body: JSON.stringify(request)
        })
        .then(response => response.json())
        .then(data => {
            const response = data as TResponse;
            successHandler(response);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public postDownloadFile(path: string, request: BaseRequestModel, successHandler: AppServiceBlobHandler, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const userToken = AppStorage.Instance.stringForKey(UICommonConstants.userTokenStorageKey);

        fetch(requestUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-IO-AUTHORIZATION': this.authorization,
                'X-IO-AUTHORIZATION-TOKEN': (userToken == null) ? '' : userToken,
                'X-IO-CLIENT-ID': this.clientID,
                'X-IO-CLIENT-SECRET': this.clientSecret
            },
            body: JSON.stringify(request)
        })
        .then(response => response.blob())
        .then(blob => {
            successHandler(blob);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public upload<TResponse extends BaseResponseModel>(path: string, blob: Blob, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const userToken = AppStorage.Instance.stringForKey(UICommonConstants.userTokenStorageKey);

        const form = new FormData();
        form.append("file", blob);

        fetch(requestUrl, {
            method: 'PUT',
            headers: {
                'X-IO-AUTHORIZATION': this.authorization,
                'X-IO-AUTHORIZATION-TOKEN': (userToken == null) ? '' : userToken,
                'X-IO-CLIENT-ID': this.clientID,
                'X-IO-CLIENT-SECRET': this.clientSecret
            },
            body: form
        })
        .then(response => response.json())
        .then(data => {
            const response = data as TResponse;
            successHandler(response);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }
}

export default AppService;