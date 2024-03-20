import ClientDeleteRequestModel from "../models/ClientDeleteRequestModel";
import React from "react";
import { BaseResponseModel, Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, QuestionView } from "iobootstrap-bo-base";

class ClientDeleteController extends Controller<{}, {}> {

    private _deleteRequest: ClientDeleteRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("clientDeleteRequest") as ClientDeleteRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("clientsList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("clientsList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_CONTROLLER_NAME}/DeleteClient`;
        const request = new ClientDeleteRequestModel();
        request.ClientId = this._deleteRequest.ClientId;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Client has been deleted successfully.", "clientsList");
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
            BreadcrumbNavigationModel.initialize("clientsList", "Clients"),
            BreadcrumbNavigationModel.initialize("clientsDelete", "Delete Client")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a client"
                    questionMessage="Are you sure want to delete this client ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default ClientDeleteController;
