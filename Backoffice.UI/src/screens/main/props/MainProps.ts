import { CalloutView, IndicatorView, ModalInputView, UploadModalView } from "iobootstrap-bo-base";
import React from "react";

interface MainProps {

    calloutView: React.RefObject<CalloutView> | undefined;
    indicatorView: React.RefObject<IndicatorView> | undefined;
    modalInputView: React.RefObject<ModalInputView> | undefined;
    uploadModalView: React.RefObject<UploadModalView> | undefined;
}

export default MainProps;