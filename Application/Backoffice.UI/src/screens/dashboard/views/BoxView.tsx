import BoxViewProps from "../props/BoxViewProps";
import React from "react";
import View from "../../../presentation/views/View";

class BoxView extends View<BoxViewProps, {}> {

    render() {
        const backgroundClass = "small-box " + this.props.backgroundStyle;
        const iconClass = "fa " + this.props.iconName;

        return (
            <React.StrictMode>
                <div className="col-lg-3 col-xs-6">
                    <div className={backgroundClass}>
                        <div className="inner">
                            <h3>{this.props.value}</h3>
                            <p>{this.props.title}</p>
                        </div>
                            <div className="icon"><i className={iconClass}></i>
                        </div>
                    </div>
                </div>
          </React.StrictMode>
        );
    }
}

export default BoxView;
