import BaseResponseModel from "../../../common/models/BaseResponseModel";
import BreadcrumbNavigationModel from "../../shared/models/BreadcrumbNavigationModel";
import CalloutTypes from "../../../presentation/constants/CalloutTypes";
import ClientUpdateRequestModel from "../models/ClientUpdateRequestModel";
import Controller from "../../../presentation/controllers/Controller";
import FormDataOptionModel from "../../shared/models/FormDataOptionModel";
import FormType from "../../shared/interfaces/FormType";
import FormTypeNumberProps from "../../shared/props/FormTypeNumberProps";
import FormTypeSelectProps from "../../shared/props/FormTypeSelectProps";
import FormTypeTextProps from "../../shared/props/FormTypeTextProps";
import FormView from "../../shared/views/FormView";
import React from "react";
import ValidationMinAmountRule from "../../../presentation/validations/ValidationMinAmountRule";
import ValidationRequiredRule from "../../../presentation/validations/ValidationRequiredRule";

class ClientUpdateController extends Controller<{}, {}> {

    private _updateRequest: ClientUpdateRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("clientUpdateRequest") as ClientUpdateRequestModel;
        if (this._updateRequest == null) {
            this.navigateToPage("clientsList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError(errorTitle: string, errorMessage: string) {
        this.calloutPresenter.show(CalloutTypes.danger, errorTitle, errorMessage);
    }

    handleFormSuccess(values: string[]) {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONTROLLER_NAME}/UpdateClient`;
        const request = new ClientUpdateRequestModel();
        request.ClientId = this._updateRequest.ClientId;
        request.IsEnabled = Number(values[0]);
        request.ClientDescription = values[1];
        request.RequestCount = Number(values[2]);
        request.MaxRequestCount = Number(values[3]);

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Client has been updated successfully.", "clientsList");
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
            BreadcrumbNavigationModel.initialize("clientsList", "Clients"),
            BreadcrumbNavigationModel.initialize("clientsUpdate", "Update Client")
        ];

        const selectValue = (this._updateRequest.IsEnabled) ? "1" : "0";
        const formElements: FormType[] = [
            FormTypeSelectProps.initialize("Is Enabled", selectValue, true, [
                FormDataOptionModel.initialize("NO", "0"),
                FormDataOptionModel.initialize("YES", "1")
            ]),
            FormTypeTextProps.initializeWithValidations("Description", this._updateRequest.ClientDescription, true, [ ValidationRequiredRule.initialize("Client name is too sort.", "Invalid client name.") ]),
            FormTypeNumberProps.initializeWithValidations("Request Count", this._updateRequest.RequestCount.toString(), true, [ ValidationMinAmountRule.initialize("Request count must be greater than 1.", "Invalid request count.", 1) ]),
            FormTypeNumberProps.initializeWithValidations("Max Request Count", this._updateRequest.MaxRequestCount.toString(), true, [ ValidationMinAmountRule.initialize("Max Request count must be greater than 1.", "Invalid max request count.", 1) ])
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Update a client"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default ClientUpdateController;
