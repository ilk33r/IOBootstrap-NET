import ConfigurationUpdateRequestModel from "../models/ConfigurationUpdateRequestModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormType, FormTypeNumberProps, FormTypeTextAreaProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class ConfigurationsUpdateController extends Controller<{}, {}> {

    private _updateRequest: ConfigurationUpdateRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("configurationUpdateRequest") as ConfigurationUpdateRequestModel;
        if (this._updateRequest == null) {
            this.navigateToPage("configurationsList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[], blobs: Blob[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/UpdateConfigItem`;
        const request = new ConfigurationUpdateRequestModel();
        request.configId = this._updateRequest.configId;
        request.configKey = values[0];
        request.intValue = (values[1] == null) ? null : Number(values[1]);
        request.strValue = (values[2] == null) ? null : values[2];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Configuration parameter has been updated successfully.", "configurationsList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        if (this._updateRequest == null) {
            return (<React.StrictMode></React.StrictMode>);
        }
    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations"),
            BreadcrumbNavigationModel.initialize("configurationsUpdate", "Update Configuration")
        ];

        const intValue = (this._updateRequest.intValue == null) ? "" : this._updateRequest.intValue.toString();
        const strValue = (this._updateRequest.strValue == null) ? "" : this._updateRequest.strValue;

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Config Key", this._updateRequest.configKey, true, [ ValidationMinLengthRule.initialize("Config key is too sort.", "Invalid config key.", 3) ]),
            FormTypeNumberProps.initialize("Integer Value", intValue, true),
            FormTypeTextAreaProps.initialize("String Value", strValue, true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Update configuration parameter"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ConfigurationsUpdateController;
