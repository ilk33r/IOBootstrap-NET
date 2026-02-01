import SaveImageResponseModel from "../models/SaveImageResponseModel";
import { BaseResponseModel, BaseView, CalloutTypes, ValidationFileTypeRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormType, FormTypeImageProps, FormView } from "iobootstrap-bo-base";

class ImagesAddController extends BOController<{}, {}> {

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
        this.generateNonce(blobs[0]);
    }

    private generateNonce(blob: Blob) {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_CONTROLLER_NAME}/GenerateNonce`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.uploadImage(blob);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private uploadImage(blob: Blob) {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_IMAGES_CONTROLLER_NAME}/SaveImage`;
        const weakSelf = this;
        
        this.service.upload(requestPath, blob, function (response: SaveImageResponseModel) {
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
            FormTypeImageProps.initializeWithValidations("Image", "", "", true, [ 
                ValidationRequiredRule.initialize("Image is required.", "Invalid image."),
                ValidationFileTypeRule.initialize("File is not valid an image.", "Invalid image.", ["jpe", "jpg", "jpeg", "png", "heic", "pjpeg"]),
            ])
        ];

        return (
            <BaseView>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add an new image"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </BaseView>
        );
    }
}

export default ImagesAddController;
