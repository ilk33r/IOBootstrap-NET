class UserInfoModel {

    id: number;
    userName: string;
    userRole: number;
    userToken: string | null;
    tokenDate: string | null;

    constructor() {
        this.id = 0;
        this.userName = "";
        this.userRole = 0;
        this.userToken = null;
        this.tokenDate = null;
    }
}

export default UserInfoModel;
