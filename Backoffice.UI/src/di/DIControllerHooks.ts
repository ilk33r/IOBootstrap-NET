import { ModalInputPresenter, ModalInputViewPresenter } from "iobootstrap-bo-base";
import { AppContext, AppServiceHeaderAuthenticationInterceptor, AppService, AppStorage, DIHooks } from "iobootstrap-ui-base";

class DIControllerHooks {

    private static appServiceHeaderInterceptor = new AppServiceHeaderAuthenticationInterceptor();
    private static appContext: AppContext = AppContext.Instance;
    private static service: AppService = AppService.Instance;
    private static storage: AppStorage = AppStorage.Instance;
    private static modalInputPresenter: ModalInputViewPresenter = ModalInputPresenter.Instance;

    static setup() {
        DIHooks.Instance.setSingletonForKey("appServiceHeaderInterceptor", DIControllerHooks.appServiceHeaderInterceptor);
        DIHooks.Instance.setSingletonForKey("appContext", DIControllerHooks.appContext);
        DIHooks.Instance.setSingletonForKey("service", DIControllerHooks.service);
        DIHooks.Instance.setSingletonForKey("storage", DIControllerHooks.storage);
        DIHooks.Instance.setSingletonForKey("modalInputPresenter", DIControllerHooks.modalInputPresenter);
    }
}

export default DIControllerHooks;