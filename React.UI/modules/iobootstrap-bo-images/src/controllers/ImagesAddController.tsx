import React from "react";
import SaveImageResponseModel from "../models/SaveImageResponseModel";
import { CalloutTypes, Controller, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormType, FormTypeImageProps, FormView } from "iobootstrap-bo-base";

class ImagesAddController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        if (errorTitle == "deleteImage" && errorMessage == "deleteImage") {
            return;
        }
        
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_IMAGES_CONTROLLER_NAME}/SaveImage`;
        const weakSelf = this;
        
        this.service.upload(requestPath, blobs[0], function (response: SaveImageResponseModel) {
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
            BreadcrumbNavigationModel.initialize("imageAdd", "Add Image")
        ];

        const formElements: FormType[] = [
            FormTypeImageProps.initializeWithValidations("Image", "", "", true, [ ValidationRequiredRule.initialize("Image is required.", "Invalid image.") ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add an new image"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ImagesAddController;
