import { BaseView, View } from "iobootstrap-ui-base";
import BreadcrumbNavigationProps from "../props/BreadcrumbNavigationProps";
import React from "react";

class BreadcrumbView extends View<BreadcrumbNavigationProps, {}> {

    render() {
        let activeNavigationName = "";
        let activeNavigationID = "";

        if (this.props.navigation.length > 0) {
            const lastItemIndex = this.props.navigation.length - 1;
            activeNavigationName = this.props.navigation[lastItemIndex].name;
            activeNavigationID = this.props.navigation[lastItemIndex].id;
        }
        
        const navigation = this.props.navigation.map(navigation => {
            const navigationId = "#!" + navigation.id;

            if (navigation.id === activeNavigationID) {
                return (<li className="breadcrumb-item active" aria-current="page" key={navigation.id}><a href={navigationId} className="icon-link-hover link-secondary link-underline-opacity-0 link-underline-opacity-75-hover">{navigation.name}</a></li>);
            }

            return (<li className="breadcrumb-item" key={navigation.id}><a href={navigationId} className="link-secondary link-underline-opacity-0 link-underline-opacity-75-hover">{navigation.name}</a></li>)
        });

        let activeNavigationTitle: React.JSX.Element;
        
        if (this.props.showTitle) {
            activeNavigationTitle = (<h2 className="page-title">{activeNavigationName}</h2>);
        } else {
            activeNavigationTitle = (<h2 className="page-title"> </h2>);
        }

        return (
            <BaseView>
                <nav aria-label="breadcrumb" className="navbar">
                    {activeNavigationTitle}
                    <ol className="breadcrumb">
                        <li key="dashboard" className="breadcrumb-item">
                            <a href="#!dashboard" className="icon-link icon-link-hover link-secondary link-underline-opacity-0 link-underline-opacity-75-hover">
                                <i className="fa fa-house fs-6 me-1 bi" aria-hidden="true"></i> {this.props.resourceHome}
                            </a>
                        </li>
                        {navigation}
                    </ol>
                </nav>
            </BaseView>
        );
    }
}

export default BreadcrumbView;
