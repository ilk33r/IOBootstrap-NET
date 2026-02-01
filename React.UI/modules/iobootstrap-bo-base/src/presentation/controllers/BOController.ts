import { BaseResponseModel, Controller, UICommonConstants } from "iobootstrap-ui-base";

class BOController<TProps, TState> extends Controller<TProps, TState> {
    
    public constructor(props: TProps) {
        super(props);
    }
    
    public handleServiceSuccess<T extends BaseResponseModel>(response: T): boolean {
        if (response.status?.code === 409) {
            this.indicatorPresenter.dismiss();
            window.location.hash = "#!userChangePassword";
            return false;
        }

        return super.handleServiceSuccess(response);
    }

    public handleServiceSuccessWithoutDismissIndicator<T extends BaseResponseModel>(response: T): boolean {
        return super.handleServiceSuccessWithoutDismissIndicator(response);
    }

    public handleInvalidCredential(response: BaseResponseModel) {
        super.handleInvalidCredential(response);
        this.storage.removeObject(UICommonConstants.userTokenStorageKey)
        this.storage.removeObject(UICommonConstants.userTokenExtrasStorageKey)
        window.location.reload();
    }
}

export default BOController;
