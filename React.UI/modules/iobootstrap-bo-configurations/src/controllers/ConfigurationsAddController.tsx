import ConfigurationAddRequestModel from "../models/ConfigurationAddRequestModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinLengthRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormType, FormTypeNumberProps, FormTypeTextAreaProps, FormTypeTextProps, FormView } from "iobootstrap-bo-base";

class ConfigurationsAddController extends Controller<{}, {}> {

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
        
        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/AddConfigItem`;
        const request = new ConfigurationAddRequestModel();
        request.configKey = values[0];
        request.intValue = (values[1] == null) ? null : Number(values[1]);
        request.strValue = (values[2] == null) ? null : values[2];

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Configuration parameter has been added successfully.", "configurationsList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations"),
            BreadcrumbNavigationModel.initialize("configurationsAdd", "Add Configuration")
        ];

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Config Key", "", true, [ ValidationMinLengthRule.initialize("Config key is too sort.", "Invalid config key.", 3) ]),
            FormTypeNumberProps.initialize("Integer Value", "", true),
            FormTypeTextAreaProps.initialize("String Value", "", true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Add a configuration parameter"
                    submitButtonName="Add"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ConfigurationsAddController;
