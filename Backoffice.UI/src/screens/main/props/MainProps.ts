import { CalloutView, IndicatorView, UploadModalView } from "iobootstrap-bo-base";
import React from "react";

interface MainProps {

    apiURL: string | undefined;
    authorization: string | undefined;
    clientID: string | undefined;
    clientSecret: string | undefined;
    calloutView: React.RefObject<CalloutView> | undefined;
    indicatorView: React.RefObject<IndicatorView> | undefined;
    uploadModalView: React.RefObject<UploadModalView> | undefined;
}

export default MainProps;