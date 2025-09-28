import { View } from "iobootstrap-ui-base";
import BoxViewProps from "../props/BoxViewProps";
import React from "react";

class BoxView extends View<BoxViewProps, {}> {

    render() {
        const backgroundClass = "small-box shadow-soft rounded-1 mb-3 " + this.props.backgroundStyle;
        const iconClass = "fas " + this.props.iconName;

        return (
            <React.StrictMode>
                <div className="col-lg-3 col-6">
                    <div className={backgroundClass}>
                        <div className="p-2">
                            <h3>{this.props.value}</h3>
                            <p>{this.props.title}</p>
                        </div>
                        <div className="icon">
                            <i className={iconClass}></i>
                        </div>
                    </div>
                </div>
          </React.StrictMode>
        );
    }
}

export default BoxView;
