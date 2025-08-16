import BreadcrumbNavigationModel from "../models/BreadcrumbNavigationModel";

class BreadcrumbNavigationProps {
    
    resourceHome: string;
    navigation: BreadcrumbNavigationModel[]
    showTitle: boolean;

    constructor() {
        this.resourceHome = "";
        this.navigation = [];
        this.showTitle = true;
    }
}

export default BreadcrumbNavigationProps;
