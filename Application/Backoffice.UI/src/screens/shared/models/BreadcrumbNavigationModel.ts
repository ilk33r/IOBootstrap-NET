class BreadcrumbNavigationModel {

    id: string;
    name: string;

    constructor() {
        this.id = "";
        this.name = "";
    }

    static initialize(id: string, name: string): BreadcrumbNavigationModel {
        let response = new BreadcrumbNavigationModel()
        response.id = id;
        response.name = name;

        return response;
    }
}

export default BreadcrumbNavigationModel;
