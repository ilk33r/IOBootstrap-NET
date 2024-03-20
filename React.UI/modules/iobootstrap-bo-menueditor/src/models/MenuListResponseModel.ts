import { BaseResponseModel } from "iobootstrap-ui-base";
import MenuListModel from "./MenuListModel";

class MenuListResponseModel extends BaseResponseModel {

    items: MenuListModel[];

    constructor() {
        super();

        this.items = [];
    }
}

export default MenuListResponseModel;
