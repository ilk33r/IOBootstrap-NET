import { ModalInputViewHandler } from "../interfaces/ModalInputViewHandler";
import ModalInputViewPresenter from "../interfaces/ModalInputViewPresenter";

class ModalInputPresenter implements ModalInputViewPresenter {

    private static _instance: ModalInputPresenter;

    public modalInputView: ModalInputViewPresenter | undefined;
    
    private constructor() {
    }

    public static get Instance() {
        return this._instance || (this._instance = new this());
    }

    public show(handler: ModalInputViewHandler): void {
        if (this.modalInputView !== undefined) {
            this.modalInputView.show(handler);
        }
    }
}

export default ModalInputPresenter;