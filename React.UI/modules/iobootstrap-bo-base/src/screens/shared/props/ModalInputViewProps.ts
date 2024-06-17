export type ModalInputViewPropsHandler = () => void;

class ModalInputViewProps {

    presentHandler: ModalInputViewPropsHandler | null;
    dismissHandler: ModalInputViewPropsHandler | null;

    constructor() {
        this.presentHandler = null;
        this.dismissHandler = null;
    }
}

export default ModalInputViewProps;