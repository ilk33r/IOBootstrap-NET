import BaseResponseModel from "../../../common/models/BaseResponseModel";
import MenuListModel from "./MenuListModel";

class MenuListResponseModel extends BaseResponseModel {

    items: MenuListModel[];

    constructor() {
        super();

        this.items = [];
    }
}

export default MenuListResponseModel;
