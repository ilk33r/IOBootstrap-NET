import AppContext from '../../core/context/AppContext';
import AppService from '../../core/service/AppService';
import AppStorage from '../../core/storage/AppStorage';
import BaseResponseModel from '../../common/models/BaseResponseModel';
import CalloutPresenter from '../presenters/CalloutPresenter';
import CalloutTypes from '../constants/CalloutTypes';
import CalloutViewPresenter from '../inerfaces/CalloutViewPresenter';
import DI from '../../di/DI';
import IndicatorPresenter from '../presenters/IndicatorPresenter';
import IndicatorViewPresenter from '../inerfaces/IndicatorViewPresenter';
import React from 'react';
import WindowMessageModel from '../../common/models/WindowMessageModel';

class Controller<TProps, TState> extends React.Component<TProps, TState> implements DI {

    public appContext: AppContext = AppContext.Instance;
    public service: AppService = AppService.Instance;
    public storage: AppStorage = AppStorage.Instance;

    public calloutPresenter: CalloutViewPresenter = CalloutPresenter.Instance;
    public indicatorPresenter: IndicatorViewPresenter = IndicatorPresenter.Instance;

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
            window.location.reload();
            return false;
        }

        this.handleServiceError(response.status?.message ?? "", response.status?.detailedMessage ?? "");
        return false;
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

    public postMessage(message: WindowMessageModel) {
        if (window.opener != null) {
            window.opener.postMessage(message, '*');
        }
    }
}

export default Controller;
