import MenuAddRequestModel from "./MenuAddRequestModel";

class MenuUpdateRequestModel extends MenuAddRequestModel {

    id: number;
    parentEntityName: string | null;

    constructor() {
        super();

        this.id = 0;
        this.parentEntityName = null;
    }
}

export default MenuUpdateRequestModel;
