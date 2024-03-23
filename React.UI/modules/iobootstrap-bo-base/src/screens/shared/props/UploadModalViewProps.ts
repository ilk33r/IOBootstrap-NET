export type UploadModalViewHandler = () => void;

class UploadModalViewProps {

    presentHandler: UploadModalViewHandler | null;
    dismissHandler: UploadModalViewHandler | null;

    constructor() {
        this.presentHandler = null;
        this.dismissHandler = null;
    }
}

export default UploadModalViewProps;