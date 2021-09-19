import AppContext from '../core/context/AppContext';
import AppService from '../core/service/AppService';
import AppStorage from '../core/storage/AppStorage';
import CalloutViewPresenter from '../presentation/inerfaces/CalloutViewPresenter';
import IndicatorViewPresenter from '../presentation/inerfaces/IndicatorViewPresenter';

interface DI {

    appContext: AppContext;
    service: AppService;
    storage: AppStorage;

    calloutPresenter: CalloutViewPresenter;
    indicatorPresenter: IndicatorViewPresenter;
}

export default DI;
