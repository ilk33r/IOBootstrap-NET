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
        this.getAsync(requestUrl)
            .then(data => {
                const response = data as TResponse;
                successHandler(response);
            })
            .catch(errorData => {
                const response = errorData as { message: string }
                errorHandler(response.message);
            });
    }

    public async getAsync(requestUrl: string): Promise<any> {
        let headers = await this.appServiceHeaderInterceptor.interceptRequestHeaders();
        headers['Content-Type'] = 'application/json';

        return fetch(requestUrl, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        })
        .then(response => {
            this.appServiceHeaderInterceptor.interceptResponseHeaders(response.headers);
            return response.json()
        });
    }

    public post<TResponse extends BaseResponseModel>(path: string, request: BaseRequestModel, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        this.postAsync(requestUrl, request)
            .then(data => {
                const response = data as TResponse;
                successHandler(response);
            })
            .catch(errorData => {
                const response = errorData as { message: string }
                errorHandler(response.message);
            });
    }

    public async postAsync(requestUrl: string, request: BaseRequestModel): Promise<any> {
        let headers = await this.appServiceHeaderInterceptor.interceptRequestHeaders();
        headers['Content-Type'] = 'application/json';

        return fetch(requestUrl, {
            method: 'POST',
            headers: headers,
            credentials: 'include',
            body: JSON.stringify(request)
        })
        .then(response => {
            this.appServiceHeaderInterceptor.interceptResponseHeaders(response.headers);
            return response.json()
        });
    }

    public downloadFile(path: string, successHandler: AppServiceBlobHandler, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        this.downloadFileAsync(requestUrl)
        .then(blob => {
            successHandler(blob);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public async downloadFileAsync(requestUrl: string): Promise<any> {
        let headers = await this.appServiceHeaderInterceptor.interceptRequestHeaders();
        headers['Content-Type'] = 'application/json';

        return fetch(requestUrl, {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        })
        .then(response => {
            this.appServiceHeaderInterceptor.interceptResponseHeaders(response.headers);
            return response.blob()
        });
    }

    public postDownloadFile(path: string, request: BaseRequestModel, successHandler: AppServiceBlobHandler, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        this.postDownloadFileAsync(requestUrl, request)
        .then(blob => {
            successHandler(blob);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public async postDownloadFileAsync(requestUrl: string, request: BaseRequestModel): Promise<any> {
        let headers = await this.appServiceHeaderInterceptor.interceptRequestHeaders();
        headers['Content-Type'] = 'application/json';

        return fetch(requestUrl, {
            method: 'POST',
            headers: headers,
            credentials: 'include',
            body: JSON.stringify(request)
        })
        .then(response => {
            this.appServiceHeaderInterceptor.interceptResponseHeaders(response.headers);
            return response.blob()
        });
    }

    public upload<TResponse extends BaseResponseModel>(path: string, blob: Blob, successHandler: AppServiceSuccessHandler<TResponse>, errorHandler: AppServiceErrorHandler) {
        const requestUrl = `${this.baseUrl}/${path}`;
        this.uploadAsync(requestUrl, blob)
        .then(data => {
            const response = data as TResponse;
            successHandler(response);
        })
        .catch(errorData => {
            const response = errorData as { message: string }
            errorHandler(response.message);
        });
    }

    public async uploadAsync(requestUrl: string, blob: Blob): Promise<any> {
        let headers = await this.appServiceHeaderInterceptor.interceptRequestHeaders();

        const form = new FormData();
        form.append("file", blob);

        return fetch(requestUrl, {
            method: 'PUT',
            headers: headers,
            credentials: 'include',
            body: form
        })
        .then(response => {
            this.appServiceHeaderInterceptor.interceptResponseHeaders(response.headers);
            return response.json()
        });
    }
}

export default AppService;