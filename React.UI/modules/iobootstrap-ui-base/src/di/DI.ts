import AppContext from '../core/context/AppContext';
import AppService from '../core/service/AppService';
import AppStorage from '../core/storage/AppStorage';
import type { CalloutViewPresenter } from '../presentation/inerfaces/CalloutViewPresenter';
import type { IndicatorViewPresenter } from '../presentation/inerfaces/IndicatorViewPresenter';

export interface DI {

    appContext: AppContext;
    service: AppService;
    storage: AppStorage;

    calloutPresenter: CalloutViewPresenter;
    indicatorPresenter: IndicatorViewPresenter;
}
