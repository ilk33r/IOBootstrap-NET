import React from 'react';
import View from '../../../presentation/views/View';

class Footer extends View<{}, {}> {

    render() {        
        return (
            <React.StrictMode>
                <footer className="main-footer">
                    <div className="pull-right hidden-xs">
                        <b>Version</b> {process.env.REACT_APP_VERSION}
                    </div>
                    <strong>Copyright &copy; 2021.</strong> All rights reserved.
                </footer>
            </React.StrictMode>
        );
    }
}

export default Footer;
