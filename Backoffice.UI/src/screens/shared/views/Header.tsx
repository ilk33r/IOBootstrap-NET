/* eslint-disable jsx-a11y/anchor-is-valid */
import HeaderProps from '../props/HeaderProps';
import HeaderState from '../props/HeaderState';
import React from 'react';
import View from '../../../presentation/views/View';

class Header extends View<HeaderProps, HeaderState> {

    constructor(props: HeaderProps) {
        super(props);

        this.handleSidebarToggleClick = this.handleSidebarToggleClick.bind(this);
    }

    private handleSidebarToggleClick(event: { target: {  }; }) {
        const collapsedClassName = 'sidebar-collapse';
        const isOpen = !$('body').hasClass(collapsedClassName);
    
        if (!isOpen) {
            $('body').removeClass(collapsedClassName);
        } else {
            $('body').addClass(collapsedClassName);
        }
    }

    render() {
        return (
            <React.StrictMode>
                <header className="main-header">
                    <a href="./" className="logo">
                        <span className="logo-mini"><b>{process.env.REACT_APP_APP_NAME}</b></span>
                        <span className="logo-lg"><b>{process.env.REACT_APP_APP_NAME}</b></span>
                    </a>
                    <nav className="navbar navbar-static-top">
                        <a href="#" className="sidebar-toggle" data-toggle="push-menu" role="button" onClick={this.handleSidebarToggleClick} >
                            <i className="fa fa-bars"></i>
                            <span className="sr-only">Toggle navigation</span>
                        </a>
                        <div className="navbar-custom-menu">
                            <ul className="nav navbar-nav">
                                <li className="dropdown user user-menu">
                                    <a href="#" className="dropdown-toggle" data-toggle="dropdown">
                                        <span className="hidden-xs">{this.props.userName}</span>
                                    </a>
                                    <ul className="dropdown-menu">
                                        <li className="user-header"><p>{this.props.userName}</p></li>
                                        <li className="user-footer">
                                            <div className="pull-left">
                                                <a href="#!userChangePassword" className="btn btn-default btn-flat">Change Password</a>
                                            </div>
                                            <div className="pull-right">
                                                <a href="#!usersLogout" className="btn btn-default btn-flat">Sign out</a>
                                            </div>
                                        </li>
                                    </ul>
                                </li>
                            </ul>
                        </div>
                    </nav>
                </header>
            </React.StrictMode>
        );
    }
}

export default Header;
