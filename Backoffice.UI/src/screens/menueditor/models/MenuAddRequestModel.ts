import BaseRequestModel from "../../../common/models/BaseRequestModel";

class MenuAddRequestModel extends BaseRequestModel {

    action: string;
    cssClass: string | null;
    name: string;
    menuOrder: number;
    requiredRole: number;
    parentEntityID: number | null;

    constructor() {
        super();
        this.action = "";
        this.cssClass = null;
        this.name = "";
        this.menuOrder = 0;
        this.requiredRole = 0;
        this.parentEntityID = null;
    }
}

export default MenuAddRequestModel;
