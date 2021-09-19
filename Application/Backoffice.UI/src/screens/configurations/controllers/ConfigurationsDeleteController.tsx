import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import ConfigurationDeleteRequestModel from "../models/ConfigurationDeleteRequestModel";
import Controller from "../../../presentation/controllers/Controller";
import QuestionView from "../../shared/views/QuestionView";
import React from "react";

class ConfigurationsDeleteController extends Controller<{}, {}> {

    private _deleteRequest: ConfigurationDeleteRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("configurationDeleteRequest") as ConfigurationDeleteRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("configurationsList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("configurationsList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/DeleteConfigItem`;
        const request = new ConfigurationDeleteRequestModel();
        request.configId = this._deleteRequest.configId;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Configuration has been deleted successfully.", "configurationsList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {
        if (this._deleteRequest == null) {
            return (<React.StrictMode></React.StrictMode>);
        }
    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations"),
            BreadcrumbNavigationModel.initialize("configurationsDelete", "Delete Configuration")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a configuration parameter"
                    questionMessage="Are you sure want to delete this configuration ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default ConfigurationsDeleteController;
