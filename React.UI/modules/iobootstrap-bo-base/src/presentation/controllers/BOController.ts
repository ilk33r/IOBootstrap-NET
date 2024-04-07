import { BaseResponseModel, Controller, UICommonConstants } from "iobootstrap-ui-base";

class BOController<TProps, TState> extends Controller<TProps, TState> {

    public handleInvalidCredential(response: BaseResponseModel) {
        super.handleInvalidCredential(response);
        this.storage.removeObject(UICommonConstants.userTokenStorageKey)
        window.location.reload();
    }
}

export default BOController;
