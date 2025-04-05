import NavigationProps from "../props/NavigationProps";
import NavigationState from "../props/NavigationState";
import React from "react";
import { View } from "iobootstrap-ui-base";

class NavigationView extends View<NavigationProps, NavigationState> {

    constructor(props: NavigationProps) {
        super(props);

        this.state = new NavigationState();
    }

    render() {
        return (
            <React.StrictMode>
            </React.StrictMode>
        );
    }
}

export default NavigationView;
