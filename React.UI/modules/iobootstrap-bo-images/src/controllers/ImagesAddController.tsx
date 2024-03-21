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
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_IMAGES_CONTROLLER_NAME}/SaveImageFile`;
        const weakSelf = this;

        // const request = new SaveImageRequestModel();
        // const imageSizeData = this._imageSizeData;

        // if (imageSizeData == null) {
        //     this.navigateToPage("imagesEdit");
        //     return;
        // }

        // const imageData = values[0].split("|");
        // const fileName = imageData[0];
        // const fileContent = imageData[1];
        // const headerAndContent = fileContent.split(";")
        // const contentType = headerAndContent[0].replace("data:", "");
        // const fileDataBase64 = headerAndContent[1].replace("base64,", "");
        // const fileData = atob(fileDataBase64);
        
        this.service.upload(requestPath, blobs[0], function (response: SaveImageResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                console.log("Image has been uploaded successfully.");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });

        // request.fileData = imageData[1];
        // imageSizeData.fileName = imageData[0];
        // request.sizes = [ imageSizeData ];

        // this.service.post(requestPath, request, function (response: SaveImageResponseModel) {
        //     if (weakSelf.handleServiceSuccess(response)) {
        //         weakSelf.showCalloutAndRedirectToHash("Image has been uploaded successfully.", "imagesEdit");
        //     }
        // }, function (error: string) {
        //     weakSelf.handleServiceError("", error);
        // });
        
        /*
        const imageData = new ImageVariationsModel();
        imageData.width = Number(values[0]);
        imageData.height = Number(values[1]);
        imageData.scale = Number(values[2]);
        imageData.keepRatio = Boolean(values[3]);

        this.appContext.setObjectForKey("imageData", imageData);
        this.navigateToPage("imageUploadFile");
        */
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
