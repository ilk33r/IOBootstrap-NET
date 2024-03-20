import { View } from "iobootstrap-ui-base";
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
                return (<li className="active" key={navigation.id}><a href={navigationId}>{navigation.name}</a></li>);
            }

            return (<li key={navigation.id}><a href={navigationId}>{navigation.name}</a></li>)
        })

        return (
            <React.StrictMode>
                <section className="content-header">
                    <h1>{activeNavigationName}</h1>
                    <ol className="breadcrumb">
                        <li key="dashboard"><a href="#!dashboard"><i className="fa fa-dashboard"></i> {this.props.resourceHome}</a></li>
                        {navigation}
                    </ol>
                </section>
            </React.StrictMode>
        );
    }
}

export default BreadcrumbView;
