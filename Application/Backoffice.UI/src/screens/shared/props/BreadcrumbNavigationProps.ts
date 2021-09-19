import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";

class BreadcrumbNavigationProps {
    
    resourceHome: string;
    navigation: BreadcrumbNavigationModel[]

    constructor() {
        this.resourceHome = "";
        this.navigation = [];
    }
}

export default BreadcrumbNavigationProps;
