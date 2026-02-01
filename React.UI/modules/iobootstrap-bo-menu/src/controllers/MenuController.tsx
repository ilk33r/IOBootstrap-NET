import MenuProps from '../props/MenuProps';
import MenuResponseModel from '../models/MenuResponseModel';
import MenuState from '../props/MenuState';
import React from 'react';
import $ from 'jquery';
import { BOController } from 'iobootstrap-bo-base';
import { BaseView } from 'iobootstrap-ui-base';

class MenuController extends BOController<MenuProps, MenuState> {

    constructor(props: MenuProps) {
        super(props);

        this.state = new MenuState();

        this.handleSidebarToggleClick = this.handleSidebarToggleClick.bind(this);
    }

    public componentDidMount?(): void {
        this.indicatorPresenter.present();

        if (window.innerWidth <= 768) {
            this.setState({
                isMediumDevice: true
            });
            
            const collapsedClassName = "sidebar-collapse";
            const isOpen = !$('body').hasClass(collapsedClassName);
    
            if (isOpen) {
                $('body').addClass(collapsedClassName);
            }
        }

        const requestURL = `${this.props.controllerName}/ListMenuItems`;
        const weakSelf = this;
        this.service.get<MenuResponseModel>(requestURL, function (response: MenuResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const items = (response.items === undefined || response.items == null) ? [] : response.items;

                items.forEach(element => {
                    if (element.action === weakSelf.props.pageHash) {
                        element.isExpanded = true;
                    }

                    if (!element.isExpanded) {
                        element.childItems?.forEach(childElement => {
                            if (childElement.action === weakSelf.props.pageHash) {
                                element.isExpanded = true;
                            }
                        });
                    }
                });

                weakSelf.setState({
                    menuItems: items
                });
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private handleSidebarToggleClick(event: React.MouseEvent<HTMLAnchorElement>) {
        event.preventDefault();

        const collapsedClassName = "sidebar-collapse";
        const isOpen = !$('body').hasClass(collapsedClassName);
    
        if (!isOpen) {
            this.setState({
                isMediumDevice: false
            });

            $('body').removeClass(collapsedClassName);
        } else {
            this.setState({
                isMediumDevice: true
            });

            $('body').addClass(collapsedClassName);
        }
    }

    render() {
        const menuContent = this.state.menuItems.map(item => {
            const childItems = (item.childItems === undefined || item.childItems == null) ? [] : item.childItems;
            if (childItems.length > 0) {
                const childItemUI = childItems.map(childItem => {
                    const itemId = "menu" + childItem.action;
                    const itemUrl = "#!" + childItem.action;
                    const itemClass = "fs-6 me-2 far " + childItem.cssClass;
                    return (
                        <li id={itemId} key={itemId}>
                            <a href={itemUrl} className="dropdown-item">
                                <div className="hstack gap-1">
                                    <i className={itemClass}></i>
                                    {childItem.name}
                                </div>
                            </a>
                        </li>
                    );
                });

                const itemClass = "fs-6 me-2 fa " + item.cssClass;
                const isExpanded = (!this.state.isMediumDevice && item.isExpanded) ? "true" : "false";
                const dropdownClassName = (!this.state.isMediumDevice && item.isExpanded) ? "dropdown-menu dropdown-menu-dark show" : "dropdown-menu dropdown-menu-dark";
                const autoClose = (this.state.isMediumDevice) ? "true" : "false";
                return (
                    <li className="nav-item dropdown" key={item.action}>
                        <a className="nav-link dropdown-toggle" href={`#!${item.action}`} role="button" data-bs-toggle="dropdown" aria-expanded={isExpanded} data-bs-auto-close={autoClose}>
                            <i className={itemClass}></i>
                            <span className="hide-collapsed">{item.name}</span>
                        </a>
                        <ul className={dropdownClassName}>
                            {childItemUI}
                        </ul>
                    </li>
                );
            } else {
                const itemId = "menu" + item.action;
                const itemUrl = "#!" + item.action;
                const itemClass = "fs-6 me-2 fa " + item.cssClass;
                return (
                    <li id={itemId} key={itemId} className="nav-item">
                        <a href={itemUrl} className="nav-link" role="button">
                            <i className={itemClass}></i>
                            <span className="hide-collapsed">{item.name}</span>
                        </a>
                    </li>
                );
            }
        });
        
        return (
            <BaseView>
                <nav className="navbar navbar-dark bg-dark bg-gradient flex-column align-items-start d-flex p-4 position-absolute start-0 bottom-0 z-2 overflow-visible sidebar" data-bs-theme="dark">
                    <ul className="navbar-nav mb-2 mb-lg-0">
                        <li className="nav-item">
                            <div className="hstack gap-3">
                                <a className="nav-link active hide-collapsed" aria-current="page" href="#root" onClick={(e) => e.preventDefault()}>
                                    <i className="fa fa-circle text-success fs-6 me-2"></i>{this.props.userName}
                                </a>
                                <a className="nav-link sidebar-toggle" aria-current="page" href="#root" onClick={this.handleSidebarToggleClick}>
                                    <i className="fas fa-down-left-and-up-right-to-center hide-collapsed"></i>
                                    <i className="fas fa-up-right-and-down-left-from-center hide-expanded"></i>
                                </a>
                            </div>
                        </li>
                        <li className="nav-item mb-1 mt-4 hide-collapsed">
                            <a className="nav-link disabled text-uppercase" aria-disabled="true" href="#root">Main Navigation</a>
                        </li>
                        {menuContent}
                    </ul>
                </nav>
            </BaseView>
        );
    }
}

export default MenuController;
