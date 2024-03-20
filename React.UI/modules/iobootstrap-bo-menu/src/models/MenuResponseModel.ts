import { BaseResponseModel } from "iobootstrap-ui-base";
import MenuModel from "./MenuModel";

class MenuResponseModel extends BaseResponseModel {

    items: MenuModel[] | undefined | null;
}

export default MenuResponseModel;
