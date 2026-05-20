import SaveFileResponseModel from "../models/SaveFileResponseModel";
import { BaseResponseModel, BaseView, CalloutTypes, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormType, FormTypeFileProps, FormView } from "iobootstrap-bo-base";

class FilesAddController extends BOController<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        if (errorTitle == "deleteFile" && errorMessage == "deleteFile") {
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
                weakSelf.uploadFile(blob);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private uploadFile(blob: Blob) {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_FILES_CONTROLLER_NAME}/SaveFile`;
        const weakSelf = this;

        this.service.upload(requestPath, blob, function (response: SaveFileResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("File has been uploaded successfully.", "filesEdit");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("filesEdit", "Files"),
            BreadcrumbNavigationModel.initialize("fileAdd", "Add File")
        ];

        const formElements: FormType[] = [
            FormTypeFileProps.initializeWithValidations("File", "", "", "*/*", true, [
                ValidationRequiredRule.initialize("File is required.", "Invalid file."),
            ])
        ];

        return (
            <BaseView>
                <FormView navigation={navigation}
                    resourceHome="Home"
                    title="Add a new file"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </BaseView>
        );
    }
}

export default FilesAddController;
