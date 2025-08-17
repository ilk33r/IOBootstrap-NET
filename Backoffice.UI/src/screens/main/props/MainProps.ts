import { CalloutView, IndicatorView, UploadModalView } from "iobootstrap-bo-base";
import React from "react";

interface MainProps {

    calloutView: React.RefObject<CalloutView | null> | undefined;
    indicatorView: React.RefObject<IndicatorView | null> | undefined;
    uploadModalView: React.RefObject<UploadModalView | null> | undefined;
}

export default MainProps;