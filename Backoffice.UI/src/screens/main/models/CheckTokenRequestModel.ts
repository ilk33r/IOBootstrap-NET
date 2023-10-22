import BaseRequestModel from '../../../common/models/BaseRequestModel';

class CheckTokenRequestModel extends BaseRequestModel {

    Token: string | undefined;
}

export default CheckTokenRequestModel;
