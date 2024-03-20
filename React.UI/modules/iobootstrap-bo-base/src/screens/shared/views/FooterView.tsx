import { View } from 'iobootstrap-ui-base';
import React from 'react';

class FooterView extends View<{}, {}> {

    render() {        
        return (
            <React.StrictMode>
                <footer className="main-footer">
                    <div className="pull-right hidden-xs">
                        <b>Version</b> {process.env.REACT_APP_VERSION}
                    </div>
                    <strong>Copyright &copy; 2024.</strong> All rights reserved.
                </footer>
            </React.StrictMode>
        );
    }
}

export default FooterView;
