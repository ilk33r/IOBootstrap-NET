import { BaseResponseModel, Controller, DIHooks, UICommonConstants } from "iobootstrap-ui-base";
import ModalInputViewPresenter from "../interfaces/ModalInputViewPresenter";

class BOController<TProps, TState> extends Controller<TProps, TState> {

    public modalInputPresenter: ModalInputViewPresenter;
    
    public constructor(props: TProps) {
        super(props);

        this.modalInputPresenter = DIHooks.Instance.singletonForKey("modalInputPresenter");
    }
    
    public handleInvalidCredential(response: BaseResponseModel) {
        super.handleInvalidCredential(response);
        this.storage.removeObject(UICommonConstants.userTokenStorageKey)
        window.location.reload();
    }
}

export default BOController;
