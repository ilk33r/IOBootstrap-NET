/* eslint-disable jsx-a11y/anchor-is-valid */
import { View } from 'iobootstrap-ui-base';
import HeaderProps from '../props/HeaderProps';
import HeaderState from '../props/HeaderState';
import React from 'react';

class HeaderView extends View<HeaderProps, HeaderState> {

    constructor(props: HeaderProps) {
        super(props);
    }

    render() {
        return (
            <React.StrictMode>
                <nav className="navbar navbar-expand-lg bg-body-tertiary z-3">
                    <div className="container-fluid">
                        <a className="navbar-brand" href={process.env.REACT_APP_BACKOFFICE_PAGE_URL}>
                            <h1 className="fs-5">{process.env.REACT_APP_APP_NAME}</h1>
                        </a>
                        <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                            <span className="navbar-toggler-icon"></span>
                        </button>
                        <div className="collapse navbar-collapse" id="navbarSupportedContent">
                            <ul className="navbar-nav me-auto mb-2 mb-lg-0">
                            </ul>
                            <ul className="navbar-nav mb-2 mb-lg-0">
                                <li className="nav-item dropdown">
                                    <a className="nav-link dropdown-toggle" href="#root" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                                    {this.props.userName}
                                    </a>
                                    <ul className="dropdown-menu dropdown-menu-end">
                                        <li><a className="dropdown-item" href="#!userChangePassword">Change Password</a></li>
                                        <li><hr className="dropdown-divider" /></li>
                                        <li><a className="dropdown-item" href="#!usersLogout">Sign out</a></li>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                    </div>
                </nav>
            </React.StrictMode>
        );
    }
}

export default HeaderView;
