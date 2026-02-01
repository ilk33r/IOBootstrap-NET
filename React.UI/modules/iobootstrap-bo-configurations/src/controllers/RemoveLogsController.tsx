import { BaseResponseModel, BaseView, CalloutTypes, ValidationDateRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BOController, BreadcrumbNavigationModel, FormType, FormTypeDateProps, FormView } from "iobootstrap-bo-base";
import RemoveLogsProps from "../props/RemoveLogsProps";
import RemoveLogsState from "../props/RemoveLogsState";
import RemoveLogsRequestModel from "../models/RemoveLogsRequestModel";

class RemoveLogsController extends BOController<RemoveLogsProps, RemoveLogsState> {

    constructor(props: RemoveLogsProps) {
        super(props);

        this.state = new RemoveLogsState();

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    private handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    private handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();
        this.generateNonce(values[0]);
    }

    private generateNonce(startDate: string) {
        const requestPath = `${import.meta.env.VITE_BACKOFFICE_CONTROLLER_NAME}/GenerateNonce`;
        const weakSelf = this;

        this.service.get(requestPath, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccessWithoutDismissIndicator(response)) {
                weakSelf.removeLogs(startDate);
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    private removeLogs(startDate: string) {
        const requestPath = `${import.meta.env.VITE_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/RemoveLogs`;
        const weakSelf = this;

        const requestModel = new RemoveLogsRequestModel();
        requestModel.startDate = startDate;

        this.service.post(requestPath, requestModel, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Logs have been removed.", "dashboard");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        })
    }

    render() {    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations"),
            BreadcrumbNavigationModel.initialize("removeLogs", "Remove Logs")
        ];

        const formElements: FormType[] = [
            FormTypeDateProps.initializeWithValidations("End Date", "", true, [ 
                ValidationRequiredRule.initialize("End date is required.", "Invalid end date."),
                ValidationDateRule.initialize("Invalid date.", "Invalid date.")
            ])
        ];
        
        return (
            <BaseView>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Remove Logs"
                    submitButtonName="Remove All"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </BaseView>
        );
    }
}

export default RemoveLogsController;
