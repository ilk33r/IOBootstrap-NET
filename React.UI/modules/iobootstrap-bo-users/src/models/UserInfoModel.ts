class UserInfoModel {

    id: number;
    userName: string;
    userRole: number;
    userToken: string | null;
    tokenDate: string | null;
    isActive: boolean;
    activationEndDate: string | null;
    createdBy: string | null;
    createdDate: string | null;
    updateDate: string | null;

    constructor() {
        this.id = 0;
        this.userName = "";
        this.userRole = 0;
        this.userToken = null;
        this.tokenDate = null;
        this.isActive = false;
        this.activationEndDate = null;
        this.createdBy = null;
        this.createdDate = null;
        this.updateDate = null;
    }
}

export default UserInfoModel;
