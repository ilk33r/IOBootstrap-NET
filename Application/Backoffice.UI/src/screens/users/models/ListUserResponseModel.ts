import BaseResponseModel from "../../../common/models/BaseResponseModel";
import UserInfoModel from "./UserInfoModel";

class ListUserResponseModel extends BaseResponseModel {

    users: UserInfoModel[];

    constructor() {
        super();

        this.users = [];
    }
}

export default ListUserResponseModel;
