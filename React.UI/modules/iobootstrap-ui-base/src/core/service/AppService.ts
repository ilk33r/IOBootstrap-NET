import BaseRequestModel from "../../common/models/BaseRequestModel";
import BaseResponseModel from "../../common/models/BaseResponseModel";
import { IAppServiceHeaderInterceptor } from "./IAppServiceHeaderInterceptor";
import DIHooks from "../../di/DIHooks";

type AppServiceBlobHandler = (blob: Blob) => void;
type AppServiceSuccessHandler<T extends BaseResponseModel> = (response: T) => void;
type AppServiceErrorHandler = (error: string) => void;

class AppService {

    public baseUrl: string;

    private static _instance: AppService;

    private appServiceHeaderInterceptor: IAppServiceHeaderInterceptor;

    private constructor() {
        this.baseUrl = "";
        this.appServiceHeaderInterceptor = DIHooks.Instance.singletonForKey("appServiceHeaderInterceptor");
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public get<TResponse extends BaseResponseModel>(path: string, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const headers = this.appServiceHeaderInterceptor.interceptHeaders();
        headers['Content-Type'] = 'application/json';

        fetch(requestUrl, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
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
        const headers = this.appServiceHeaderInterceptor.interceptHeaders();
        headers['Content-Type'] = 'application/json';

        fetch(requestUrl, {
            method: 'POST',
            headers: headers,
            credentials: 'include',
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

    public downloadFile(path: string, successHandler: AppServiceBlobHandler, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const headers = this.appServiceHeaderInterceptor.interceptHeaders();
        headers['Content-Type'] = 'application/json';

        fetch(requestUrl, {
            method: 'GET',
            credentials: 'include',
            headers: headers
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

    public postDownloadFile(path: string, request: BaseRequestModel, successHandler: AppServiceBlobHandler, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        const headers = this.appServiceHeaderInterceptor.interceptHeaders();
        headers['Content-Type'] = 'application/json';

        fetch(requestUrl, {
            method: 'POST',
            credentials: 'include',
            headers: headers,
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
        const headers = this.appServiceHeaderInterceptor.interceptHeaders();

        const form = new FormData();
        form.append("file", blob);

        fetch(requestUrl, {
            method: 'PUT',
            credentials: 'include',
            headers: headers,
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