import CalloutView from "../../shared/views/CalloutView";
import IndicatorView from "../../shared/views/IndicatorView";
import React from "react";

interface MainProps {

    apiURL: string | undefined;
    authorization: string | undefined;
    clientID: string | undefined;
    clientSecret: string | undefined;
    calloutView: React.RefObject<CalloutView> | undefined;
    indicatorView: React.RefObject<IndicatorView> | undefined;
}

export default MainProps;