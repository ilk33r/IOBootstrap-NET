import { BaseResponseModel } from "iobootstrap-ui-base";
import UserInfoModel from "./UserInfoModel";

class ListUserResponseModel extends BaseResponseModel {

    users: UserInfoModel[];

    constructor() {
        super();

        this.users = [];
    }
}

export default ListUserResponseModel;
