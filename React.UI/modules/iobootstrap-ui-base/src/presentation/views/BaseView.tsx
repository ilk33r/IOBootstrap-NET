import React from "react";

class BaseView extends React.PureComponent<React.PropsWithChildren> {
    render() {
        return (
            <>
                {this.props.children}
            </>
        );
    }
}

export default BaseView;
