import BaseRequestModel from '../../../common/models/BaseRequestModel';

class AuthenticationRequestModel extends BaseRequestModel {

    UserName: string | undefined;
    Password: string | undefined;
}

export default AuthenticationRequestModel;
