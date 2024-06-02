import { ModalInputViewHandler } from "./ModalInputViewHandler";

interface ModalInputViewPresenter {

    show(handler: ModalInputViewHandler): void;
}

export default ModalInputViewPresenter;