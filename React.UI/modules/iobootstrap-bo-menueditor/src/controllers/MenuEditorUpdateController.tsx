import MenuUpdateRequestModel from "../models/MenuUpdateRequestModel";
import React from "react";
import { BaseResponseModel, CalloutTypes, Controller, ValidationMinAmountRule, ValidationRequiredRule } from "iobootstrap-ui-base";
import { BreadcrumbNavigationModel, FormDataOptionModel, FormType, FormTypeNumberProps, FormTypePopupSelectionProps, FormTypeSelectProps, FormTypeTextProps, FormView, UserRoles } from "iobootstrap-bo-base";

class MenuEditorUpdateController extends Controller<{}, {}> {

    private _updateRequest: MenuUpdateRequestModel;

    constructor(props: {}) {
        super(props);

        this._updateRequest = this.appContext.objectForKey("menuEditorUpdateRequest") as MenuUpdateRequestModel;
        if (this._updateRequest == null) {
            this.navigateToPage("menuEditorList");
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

        const requestPath = `${process.env.REACT_APP_BACKOFFICE_MENU_CONTROLLER_NAME}/UpdateMenuItem`;
        const request = new MenuUpdateRequestModel();
        request.id = this._updateRequest.id;
        request.name = values[0];
        request.action = values[1];
        request.cssClass = values[2];
        request.requiredRole = (values[3] == null ) ? 0 : Number(values[3]);
        request.menuOrder = (values[4] == null ) ? 0 : Number(values[4]);
        request.parentEntityID = (values[5] == null ) ? null : Number(values[5]);

        if (request.parentEntityID === 0) {
            request.parentEntityID = null;
        }

        const weakSelf = this;
        this.service.post(requestPath, request, function (response: BaseResponseModel) {
            if (weakSelf.handleServiceSuccess(response)) {
                weakSelf.showCalloutAndRedirectToHash("Menu has been updated successfully.", "menuEditorList");
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
            BreadcrumbNavigationModel.initialize("menuEditorList", "Menu Editor"),
            BreadcrumbNavigationModel.initialize("menuEditorUpdate", "Update Menu")
        ];

        const formElements: FormType[] = [
            FormTypeTextProps.initializeWithValidations("Name", this._updateRequest.name, true, [ ValidationRequiredRule.initialize("Name is too short.", "Invalid menu name.") ]),
            FormTypeTextProps.initializeWithValidations("Action", this._updateRequest.action, true, [ ValidationRequiredRule.initialize("Action is too short.", "Invalid menu action.") ]),
            FormTypeTextProps.initialize("CSS Class Name", this._updateRequest.cssClass ?? "", true),
            FormTypeSelectProps.initialize("Required Role", this._updateRequest.requiredRole.toString(), true, [
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.SuperAdmin), UserRoles.SuperAdmin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.Admin), UserRoles.Admin.toString()),
                FormDataOptionModel.initialize(UserRoles.getRoleName(UserRoles.User), UserRoles.User.toString()),
            ]),
            FormTypeNumberProps.initializeWithValidations("Menu Order", this._updateRequest.menuOrder.toString(), true, [ 
                ValidationRequiredRule.initialize("Menu order must be required.", "Invalid menu order."),
                ValidationMinAmountRule.initialize("Menu order must be greater than zero.", "Invalid menu order.", 0)
            ]),
            FormTypePopupSelectionProps.initialize("Parent Menu", this._updateRequest.parentEntityName ?? "", this._updateRequest.parentEntityID ?? 0, "menuEditorSelect", true)
        ];

        return (
            <React.StrictMode>
                <FormView navigation={navigation} 
                    resourceHome="Home"
                    title="Update menu item"
                    submitButtonName="Save"
                    errorHandler={this.handleFormError}
                    successHandler={this.handleFormSuccess}
                    formElements={formElements} />
            </React.StrictMode>
        );
    }
}

export default MenuEditorUpdateController;
