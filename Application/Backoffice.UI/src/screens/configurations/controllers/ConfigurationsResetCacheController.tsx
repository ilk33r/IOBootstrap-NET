import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import Controller from "../../../presentation/controllers/Controller";
import QuestionView from "../../shared/views/QuestionView";
import React from "react";

class ConfigurationsResetCacheController extends Controller<{}, {}> {

    constructor(props: {}) {
        super(props);

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("configurationsList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONFIGURATION_CONTROLLER_NAME}/ResetCache`;

        const weakSelf = this;
        this.service.get(requestPath, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Application cache has been cleaned.", "configurationsList");
            }
        }, function (error: string) {
            weakSelf.handleServiceError("", error);
        });
    }

    render() {    
        const navigation: BreadcrumbNavigationModel[] = [
            BreadcrumbNavigationModel.initialize("configurationsList", "Configurations"),
            BreadcrumbNavigationModel.initialize("resetCache", "Reset Cache")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Clean application cache"
                    questionMessage="Are you sure want to clean the application cache ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default ConfigurationsResetCacheController;
