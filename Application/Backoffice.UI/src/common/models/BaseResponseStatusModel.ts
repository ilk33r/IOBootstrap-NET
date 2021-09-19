class BaseResponseStatusModel {

    code: number | undefined;
    detailedMessage: string | undefined;
    message: string | undefined;
    success: boolean | undefined;
}

export default BaseResponseStatusModel;