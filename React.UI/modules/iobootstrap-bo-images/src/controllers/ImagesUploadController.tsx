import ImageVariationsModel from "../models/ImageVariationsModel";
import React from "react";
import SaveImageRequestModel from "../models/SaveImageRequestModel";
import SaveImageResponseModel from "../models/SaveImageResponseModel";
import { CalloutTypes, Controller, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormType, FormTypeImageProps, FormView } from "iobootstrap-bo-base";

class ImagesUploadController extends Controller<{}, {}> {

    private _imageSizeData: ImageVariationsModel | null;

    constructor(props: {}) {
        super(props);

        this._imageSizeData = this.appContext.objectForKey("imageData") as ImageVariationsModel;
        if (this._imageSizeData == null) {
            this.navigateToPage("imagesEdit");
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        if (errorTitle !== "deleteImage") {
            this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
        }
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_IMAGES_CONTROLLER_NAME}/SaveImages`;
        const weakSelf = this;

        const request = new SaveImageRequestModel();
        const imageSizeData = this._imageSizeData;

        if (imageSizeData == null) {
            this.navigateToPage("imagesEdit");
            return;
        }

        const imageData = values[0].split("|");
        request.fileData = imageData[1];
        imageSizeData.fileName = imageData[0];
        request.sizes = [ imageSizeData ];

        this.service.post(requestPath, request, function (response: SaveImageResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Image has been uploaded successfully.", "imagesEdit");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("imagesEdit", "Images"),
            BreadcrumbNavigationModel.initialize("imageAdd", "Add Image"),
            BreadcrumbNavigationModel.initialize("imageUploadFile", "Upload File"),
        ];

        const formElements: FormType[] = [
            FormTypeImageProps.initializeWithValidations("Image", "", "", true, [ ValidationRequiredRule.initialize("Image is required.", "Invalid image.") ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add an new image"
                    submitButtonName="Upload"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ImagesUploadController;
