/* eslint-disable jsx-a11y/anchor-is-valid */
import Controller from '../../../presentation/controllers/Controller';
import MenuProps from '../props/MenuProps';
import MenuResponseModel from '../models/MenuResponseModel';
import MenuState from '../props/MenuState';
import React from 'react';
import MenuModel from '../models/MenuModel';

class Menu extends Controller<MenuProps, MenuState> {

    constructor(props: MenuProps) {
        super(props);

        this.state = new MenuState();

        this.handleParentMenuClick = this.handleParentMenuClick.bind(this);
    }

    public componentDidMount?(): void {
        this.indicatorPresenter.present();

        const requestURL = `${this.props.controllerName}/ListMenuItems`;
        const weakSelf = this;
        this.service.get<MenuResponseModel>(requestURL, function (response: MenuResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                const newState = new MenuState();
                const items = (response.items === undefined || response.items == null) ? [] : response.items;
                newState.menuItems = items;
                weakSelf.setState(newState);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private handleParentMenuClick(event: { target: {  }; }, item: MenuModel) {
        const menuItems = this.state.menuItems;
        menuItems.forEach(element => {
            if (element.action === item.action) {
                element.isExpanded = !element.isExpanded;
            }
        });

        const newState = new MenuState();
        newState.menuItems = menuItems;
        this.setState(newState);
    }

    render() {
        const menuContent = this.state.menuItems.map(item => {
            const childItems = (item.childItems === undefined || item.childItems == null) ? [] : item.childItems;
            if (childItems.length > 0) {
                const childItemUI = childItems.map(childItem => {
                    const itemId = "menu" + childItem.action;
                    const itemUrl = "#!" + childItem.action;
                    const itemClass = "far " + childItem.cssClass;
                    return (<li id={itemId} key={itemId}><a href={itemUrl}><i className={itemClass}></i> {childItem.name}</a></li>);
                });

                const itemClass = "fa " + item.cssClass;
                const parentItemClassName = (item.isExpanded) ? "treeview menu-open active" : "treeview";
                return (
                    <li className={parentItemClassName} key={item.action}>
                        <a href="#" onClick={(e) => this.handleParentMenuClick(e, item)}>
                            <i className={itemClass}></i> <span>{item.name}</span>
                            <span className="pull-right-container"><i className="fa fa-angle-left pull-right"></i></span>
                        </a>
                        <ul className="treeview-menu">
                            {childItemUI}
                        </ul>
                    </li>
                );
            } else {
                const itemId = "menu" + item.action;
                const itemUrl = "#!" + item.action;
                const itemClass = "fa " + item.cssClass;
                return (<li id={itemId} key={itemId}><a href={itemUrl}><i className={itemClass}></i> <span>{item.name}</span></a></li>);
            }
        });
        
        return (
            <React.StrictMode>
                <aside className="main-sidebar">
                    <section className="sidebar">
                        <div className="user-panel">
                            <div className="pull-left image">
                                <div><br /></div>
                                <div><br /></div>
                                <div><br /></div>
                            </div>
                            <div className="pull-left info text-center">
                            <p>{this.props.userName}</p>
                                <a><i className="fa fa-circle text-success"></i> Online</a>
                            </div>
                        </div>
                        <ul className="sidebar-menu" data-widget="tree">
                            <li className="header">MAIN NAVIGATION</li>
                            {menuContent}
                        </ul>
                    </section>
                </aside>
            </React.StrictMode>
        );
    }
}

export default Menu;
