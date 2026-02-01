import { BaseResponseModel } from "iobootstrap-ui-base";
import UserInfoModel from "./UserInfoModel";

class ListUserResponseModel extends BaseResponseModel {

    count: number | null;
    users: UserInfoModel[];

    constructor() {
        super();

        this.count = null;
        this.users = [];
    }
}

export default ListUserResponseModel;
