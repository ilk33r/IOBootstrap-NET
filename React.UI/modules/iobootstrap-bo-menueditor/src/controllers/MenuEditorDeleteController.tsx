import MenuDeleteRequestModel from "../models/MenuDeleteRequestModel";
import React from "react";
import { BaseResponseModel, Controller } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, QuestionView } from "iobootstrap-bo-base";

class MenuEditorDeleteController extends Controller<{}, {}> {

    private _deleteRequest: MenuDeleteRequestModel;

    constructor(props: {}) {
        super(props);

        this._deleteRequest = this.appContext.objectForKey("menuEditorDeleteRequest") as MenuDeleteRequestModel;
        if (this._deleteRequest == null) {
            this.navigateToPage("menuEditorList");
            return;
        }

        this.handleFormError = this.handleFormError.bind(this);
        this.handleFormSuccess = this.handleFormSuccess.bind(this);
    }

    handleFormError() {
        this.navigateToPage("menuEditorList");
    }

    handleFormSuccess() {
        this.indicatorPresenter.present();

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME}/DeleteMenuItem`;
        const request = new MenuDeleteRequestModel();
        request.id = this._deleteRequest.id;

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Menu has been deleted successfully.", "menuEditorList");
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
            BreadcrumbNavigationModel.initialize("menuEditorList", "Menu Editor"),
            BreadcrumbNavigationModel.initialize("menuEditorDelete", "Delete Menu")
        ];

        return (
            <React.StrictMode>
                <QuestionView navigation={navigation} 
                    resourceHome="Home"
                    title="Delete a menu item"
                    questionMessage="Are you sure want to delete this menu item ?"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                     />
            </React.StrictMode>
        );
    }
}

export default MenuEditorDeleteController;
