import DeleteImagesRequestModel from "../models/DeleteImagesRequestModel";
import ImageVariationsModel from "../models/ImageVariationsModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinAmountRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeImageProps, FormTypeNumberProps, FormTypeSelectProps, FormView } from "iobootstrap-bo-base";

class ImagesModifyController extends Controller<{}, {}> {

    private _selectedImage: ImageVariationsModel | null;

    constructor(props: {}) {
        super(props);

        this._selectedImage = this.appContext.objectForKey("selectedImage") as ImageVariationsModel;
        if (this._selectedImage == null) {
            this.navigateToPage("imagesEdit");
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    private deleteImage() {
        this.indicatorPresenter.present();

        const imageId = (this._selectedImage?.id !== undefined && this._selectedImage?.id != null) ? this._selectedImage?.id : 0;
        const request = new DeleteImagesRequestModel();
        request.imageId = imageId;

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_IMAGES_CONTROLLER_NAME}/DeleteImage`;
        const weakSelf = this;

        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.navigateToPage("imagesEdit");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        if (errorTitle == "deleteImage" && errorMessage == "deleteImage") {
            this.deleteImage();
            return;
        }

        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {

    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("imagesEdit", "Images"),
            BreadcrumbNavigationModel.initialize("imageModify", "Modify Image")
        ];

        const imageWidth = (this._selectedImage?.width !== undefined && this._selectedImage?.width != null) ? this._selectedImage?.width : 0;
        const imageHeight = (this._selectedImage?.height !== undefined && this._selectedImage?.height != null) ? this._selectedImage?.height : 0;
        const imageScale = (this._selectedImage?.scale !== undefined && this._selectedImage?.scale != null) ? this._selectedImage?.scale : 0;
        const keepRatio = (this._selectedImage?.keepRatio !== undefined && this._selectedImage?.keepRatio != null) ? this._selectedImage?.keepRatio : true;
        const keepRationValue = (keepRatio) ? "1" : "0";
        const imageFileName = (this._selectedImage?.fileName !== undefined && this._selectedImage?.fileName != null) ? this._selectedImage?.fileName : "";
        const imageUrl = `${process.env.REACT_APP_API_URL}/${process.env.REACT_APP_IMAGE_ASSETS_CONTROLLER}/Get?publicId=${encodeURIComponent(imageFileName)}`

        const formElements: FormType[] = [
            FormTypeNumberProps.initializeWithValidations("Width", imageWidth.toString(), false, [ ValidationMinAmountRule.initialize("Width must be greater than 0.", "Invalid image width.", 0) ]),
            FormTypeNumberProps.initializeWithValidations("Height", imageHeight.toString(), false, [ ValidationMinAmountRule.initialize("Height must be greater than 0.", "Invalid image height.", 0) ]),
            FormTypeNumberProps.initializeWithValidations("Scale", imageScale.toString(), false, [ ValidationMinAmountRule.initialize("Scale must be greater than 0.", "Invalid image scale.", 0) ]),
            FormTypeSelectProps.initialize("Keep Ratio", keepRationValue, false, [
                FormDataOptionModel.initialize("No", "0"),
                FormDataOptionModel.initialize("YES", "1")
            ]),
            FormTypeImageProps.initializeWithValidations("Image", imageUrl, imageFileName, true, [ ValidationRequiredRule.initialize("Image is required.", "Invalid image.") ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add an new image"
                    submitButtonName=""
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ImagesModifyController;
