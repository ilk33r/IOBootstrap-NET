import AppContext from '../../core/context/AppContext';
import AppService from '../../core/service/AppService';
import AppStorage from '../../core/storage/AppStorage';
import BaseResponseModel from '../../common/models/BaseResponseModel';
import CalloutPresenter from '../presenters/CalloutPresenter';
import CalloutTypes from '../constants/CalloutTypes';
import type { CalloutViewPresenter } from '../inerfaces/CalloutViewPresenter';
import type { DI } from '../../di/DI';
import IndicatorPresenter from '../presenters/IndicatorPresenter';
import type { IndicatorViewPresenter } from '../inerfaces/IndicatorViewPresenter';
import React from 'react';
import type { WindowMessageModel } from '../../common/models/WindowMessageModel';
import { UploadModalViewPresenter } from '../inerfaces/UploadModalViewPresenter';
import UploadModalPresenter from '../presenters/UploadModalPresenter';

class Controller<TProps, TState> extends React.Component<TProps, TState> implements DI {

    public appContext: AppContext = AppContext.Instance;
    public service: AppService = AppService.Instance;
    public storage: AppStorage = AppStorage.Instance;

    public calloutPresenter: CalloutViewPresenter = CalloutPresenter.Instance;
    public indicatorPresenter: IndicatorViewPresenter = IndicatorPresenter.Instance;
    public uploadModalPresenter: UploadModalViewPresenter = UploadModalPresenter.Instance;

    public componentDidMount?(): void {
    }

    public componentWillUnmount?(): void {
    }

    public handleServiceError(title: string, message: string) {
        this.indicatorPresenter.dismiss();
        this.calloutPresenter.show(CalloutTypes.danger, title, message);
    }

    public handleServiceSuccess<T extends BaseResponseModel>(response: T): boolean {
        this.indicatorPresenter.dismiss();
        
        if (response.status?.code === 200) {
            return true;
        }

        if (response.status?.code === 401 || response.status?.code === 403) {
            this.handleInvalidCredential(response);
            return false;
        }

        if (response.status?.code === 630) {
            this.handleInvalidKeyID(response);
            return false;
        }

        this.handleServiceError(response.status?.message ?? "", response.status?.detailedMessage ?? "");
        return false;
    }

    public handleInvalidCredential(response: BaseResponseModel) {
        this.handleServiceError(response.status?.message ?? "", response.status?.detailedMessage ?? "");
    }

    public handleInvalidKeyID(response: BaseResponseModel) {
        window.location.reload();
    }

    public showCalloutAndRedirectToHash(successMessage: string, hash: string) {
        this.calloutPresenter.show(CalloutTypes.success, "", successMessage);

        setTimeout(function () {
            window.location.hash = "#!" + hash;
        }, 350);
    }

    public navigateToPage(pageHash: string) {
        setTimeout(function () {
            window.location.hash = "#!" + pageHash;
        }, 350);
    }

    public postMessage(name: string, itemID: number | null, itemValue: string | null) {
        const windowMessage: WindowMessageModel = {
            name: name,
            itemID: itemID, 
            itemValue: itemValue
        };
        window.postMessage(windowMessage, '*');
    }

    public downloadFile(blob: Blob, fileName: string) {
        const objectUrl: string = URL.createObjectURL(blob);
        const anchor: HTMLAnchorElement = document.createElement('a') as HTMLAnchorElement;
    
        anchor.href = objectUrl;
        anchor.download = fileName;
        document.body.appendChild(anchor);
        anchor.click();
    
        document.body.removeChild(anchor);
        URL.revokeObjectURL(objectUrl);
    }
}

export default Controller;
