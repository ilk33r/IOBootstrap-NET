import BaseResponseModel from "../../../common/models/BaseResponseModel";
import MenuModel from "./MenuModel";

class MenuResponseModel extends BaseResponseModel {

    items: MenuModel[] | undefined | null;
}

export default MenuResponseModel;
