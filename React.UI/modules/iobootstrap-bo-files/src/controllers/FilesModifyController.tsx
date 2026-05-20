import DeleteFilesRequestModel from "../models/DeleteFilesRequestModel";
import FileVariationsModel from "../models/FileVariationsModel";
import { BaseResponseModel, BaseView, CalloutTypes, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormType, FormTypeFileProps, FormView } from "iobootstrap-bo-base";

class FilesModifyController extends BOController<{}, {}> {

    private _selectedFile: FileVariationsModel | null;

    constructor(props: {}) {
        super(props);

        this._selectedFile = this.appContext.objectForKey("selectedFile") as FileVariationsModel;
        if (this._selectedFile == null) {
            this.navigateToPage("filesEdit");
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        if (errorTitle == "deleteFile" && errorMessage == "deleteFile") {
            this.generateNonce();
            return;
        }

        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {

    }

    private generateNonce() {
        this.indicatorPresenter.present();

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_CONTROLLER_NAME}/GenerateNonce`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.deleteFile();
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private deleteFile() {
        this.indicatorPresenter.present();

        const fileId = (this._selectedFile?.id !== undefined && this._selectedFile?.id != null) ? this._selectedFile?.id : 0;
        const request = new DeleteFilesRequestModel();
        request.fileId = fileId;

        const requestPath = `${import.meta.env.VITE_BACKOFFICE_FILES_CONTROLLER_NAME}/DeleteFile`;
        const weakSelf = this;

        this.service.delete(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.navigateToPage("filesEdit");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("filesEdit", "Files"),
            BreadcrumbNavigationModel.initialize("fileModify", "Modify File")
        ];

        const fileName = (this._selectedFile?.fileName !== undefined && this._selectedFile?.fileName != null) ? this._selectedFile?.fileName : "";

        const formElements: FormType[] = [
            FormTypeFileProps.initializeWithValidations("File", fileName, fileName, "*/*", true, [
                ValidationRequiredRule.initialize("File is required.", "Invalid file."),
            ])
        ];

        return (
            <BaseView>
                <FormView navigation={navigation}
                    resourceHome="Home"
                    title="Modify file"
                    submitButtonName=""
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </BaseView>
        );
    }
}

export default FilesModifyController;
